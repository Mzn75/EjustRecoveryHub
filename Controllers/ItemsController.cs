using EjustRecoveryHub.Data;
using EjustRecoveryHub.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace EjustRecoveryHub.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _protector;

        public ItemsController(ApplicationDbContext context, IDataProtectionProvider provider)
        {
            _context = context;
            _protector = provider.CreateProtector("ReportedItemsCookieLock");
        }

        [HttpGet]
        public async Task<IActionResult> FoundItems(string filter = "recent")
        {
            // 1. Setup Ownership Cookies
            string userReportedIds = Request.Cookies["MyReportedItems"] ?? "";
            var idList = string.IsNullOrEmpty(userReportedIds)
                ? new List<int>()
                : userReportedIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

            ViewBag.MyReportedIds = idList;
            ViewBag.CurrentFilter = filter;

            // 2. Query ALL Active Items
            var query = _context.Items.Where(i => i.Status == "Active").AsQueryable();

            // 3. Filter by Specific Database Tables instead of a Category string
            if (!string.IsNullOrEmpty(filter) && filter != "recent" && filter != "active")
            {
                var f = filter.ToLower();
                if (f == "device") query = query.OfType<DeviceItem>();
                else if (f == "id") query = query.OfType<IdItem>();
                else if (f == "wallet") query = query.OfType<WalletItem>();
                else if (f == "jewelry") query = query.OfType<JewelryItem>();
                else if (f == "notebook") query = query.OfType<NotebookItem>();
            }

            // 4. Sort and Execute Async
            if (filter == "active")
            {
                query = query.OrderBy(i => i.Status).ThenByDescending(i => i.DateReported);
            }
            else
            {
                query = query.OrderByDescending(i => i.DateReported);
            }

            ViewBag.RecentItems = await query.Take(10).ToListAsync();

            // Separately grab just their items for the "My Reports" section
            ViewBag.MyItems = await _context.Items.Where(i => idList.Contains(i.Id)).ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFoundItem(ItemViewModel form)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RecentItems = await _context.Items.Where(i => i.Status == "Active").OrderByDescending(i => i.DateReported).Take(10).ToListAsync();
                return View("FoundItems", form);
            }

            ItemModel newDbItem = null;
            bool isDuplicate = false;
            DateTime timeThreshold = DateTime.UtcNow.AddHours(-24);

            // Map the ViewModel form to the strict Relational Database objects
            if (!string.IsNullOrEmpty(form.Category))
            {
                switch (form.Category.ToLower())
                {
                    case "id":
                        isDuplicate = await _context.IdItems.AnyAsync(i => i.IdNumber == form.IdNumber);
                        newDbItem = new IdItem
                        {
                            Category = "id",
                            ContactNumber = form.ContactNumber,
                            ContactEmail = form.ContactEmail,
                            LocationFound = form.LocationFound,
                            ItemLocation = form.ItemLocation,
                            IdName = form.IdName,
                            IdNumber = form.IdNumber
                        };
                        break;

                    case "device":
                        isDuplicate = await _context.DeviceItems.AnyAsync(i =>
                            i.DateReported >= timeThreshold &&
                            i.DeviceBrand == form.DeviceBrand &&
                            i.DeviceModel == form.DeviceModel);
                        newDbItem = new DeviceItem
                        {
                            Category = "device",
                            ContactNumber = form.ContactNumber,
                            ContactEmail = form.ContactEmail,
                            LocationFound = form.LocationFound,
                            ItemLocation = form.ItemLocation,
                            DeviceBrand = form.DeviceBrand,
                            DeviceModel = form.DeviceModel,
                            DeviceDescription = form.DeviceDescription
                        };
                        break;

                    case "wallet":
                        isDuplicate = await _context.WalletItems.AnyAsync(i =>
                            i.DateReported >= timeThreshold &&
                            i.WalletColor == form.WalletColor &&
                            i.LocationFound == form.LocationFound);
                        newDbItem = new WalletItem
                        {
                            Category = "wallet",
                            ContactNumber = form.ContactNumber,
                            ContactEmail = form.ContactEmail,
                            LocationFound = form.LocationFound,
                            ItemLocation = form.ItemLocation,
                            WalletColor = form.WalletColor,
                            WalletBrandOrMaterial = form.WalletBrandOrMaterial
                        };
                        break;

                    case "jewelry":
                        isDuplicate = await _context.JewelryItems.AnyAsync(i =>
                            i.DateReported >= timeThreshold &&
                            i.JewelryType == form.JewelryType &&
                            i.LocationFound == form.LocationFound);
                        newDbItem = new JewelryItem
                        {
                            Category = "jewelry",
                            ContactNumber = form.ContactNumber,
                            ContactEmail = form.ContactEmail,
                            LocationFound = form.LocationFound,
                            ItemLocation = form.ItemLocation,
                            JewelryType = form.JewelryType,
                            JewelryMaterial = form.JewelryMaterial
                        };
                        break;

                    case "notebook":
                        isDuplicate = await _context.NotebookItems.AnyAsync(i =>
                            i.DateReported >= timeThreshold &&
                            i.NotebookColor == form.NotebookColor &&
                            i.LocationFound == form.LocationFound);
                        newDbItem = new NotebookItem
                        {
                            Category = "notebook",
                            ContactNumber = form.ContactNumber,
                            ContactEmail = form.ContactEmail,
                            LocationFound = form.LocationFound,
                            ItemLocation = form.ItemLocation,
                            NotebookColor = form.NotebookColor
                        };
                        break;
                }
            }

            if (isDuplicate || newDbItem == null)
            {
                TempData["DuplicateMessage"] = "This item has already been reported or the category is invalid.";
                ViewBag.Duplicate = true;
                return View("FoundItems", form);
            }

            // Secure Photo Upload
            if (form.ItemPhoto != null && form.ItemPhoto.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(form.ItemPhoto.FileName).ToLower();

                if (!allowedExtensions.Contains(extension) || form.ItemPhoto.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("ItemPhoto", "Please upload a valid image (JPG/PNG) under 5MB.");
                    return View("FoundItems", form);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + extension;
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await form.ItemPhoto.CopyToAsync(fileStream);
                    }
                    newDbItem.PhotoPath = "/uploads/" + uniqueFileName;
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "An error occurred while uploading the photo.");
                    return View("FoundItems", form);
                }
            }

            newDbItem.DateReported = DateTime.UtcNow;
            _context.Items.Add(newDbItem);
            await _context.SaveChangesAsync();

            // Setup Browser Cookie
            // 1. Get existing secure IDs using the helper method we created
            List<int> myReportedIds = GetDecryptedCookieIds();

            // 2. Add the new item's integer ID
            myReportedIds.Add(newDbItem.Id);

            // 3. Serialize to JSON and Encrypt!
            string jsonString = JsonSerializer.Serialize(myReportedIds);
            string encryptedString = _protector.Protect(jsonString);

            // 4. Save the locked cookie
            Response.Cookies.Append("MyReportedItems", encryptedString, new CookieOptions
            {
                HttpOnly = true, // Prevents JavaScript from reading the cookie
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });

            TempData["SuccessMessage"] = "Item successfully reported! Thank you for helping.";
            return RedirectToAction("FoundItems");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            // 1. Strict Whitelist validation
            if (newStatus != "Active" && newStatus != "Returned")
            {
                TempData["DuplicateMessage"] = "Error: Invalid status requested.";
                return RedirectToAction("FoundItems");
            }

            // 2. Safe Cookie Parsing
            List<int> authorizedIdList = GetDecryptedCookieIds();

            // 3. Security Check (IDOR Protection)
            if (!authorizedIdList.Contains(id))
            {
                TempData["DuplicateMessage"] = "Security Alert: You do not have permission to update this item.";
                return RedirectToAction("FoundItems");
            }

            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id);
            if (item != null)
            {
                item.Status = newStatus;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Item status successfully updated to {newStatus}!";
            }

            return RedirectToAction("FoundItems");
        }

        private List<int> GetDecryptedCookieIds()
        {
            var encryptedCookie = Request.Cookies["MyReportedItems"];
            if (string.IsNullOrEmpty(encryptedCookie))
            {
                return new List<int>(); // No cookie found
            }

            try
            {
                // Attempts to unlock the data
                var decryptedJson = _protector.Unprotect(encryptedCookie);
                return JsonSerializer.Deserialize<List<int>>(decryptedJson) ?? new List<int>();
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                // 🚨 A hacker tampered with the cookie text in DevTools!
                // The Unprotect method automatically throws an exception if the signature doesn't match.
                return new List<int>();
            }
        }

        [HttpGet]
        public IActionResult LostItems()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RevealContact(int id)
        {
            // MISSING RATE LIMITING RESTORED
            string attemptsStr = Request.Cookies["RevealAttempts"] ?? "0";
            int.TryParse(attemptsStr, out int attempts);

            if (attempts >= 3)
            {
                return Json(new { success = false, message = "For privacy, you can only reveal 3 contact informations per day. Please try again tomorrow." });
            }

            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id);

            if (item == null || string.IsNullOrEmpty(item.ContactNumber) && string.IsNullOrEmpty(item.ContactEmail))
            {
                return Json(new { success = false, message = "Contact info not available." });
            }

            // Update Rate Limiting Cookie
            Response.Cookies.Append("RevealAttempts", (attempts + 1).ToString(), new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddHours(24)
            });

            return Json(new { success = true, phone = item.ContactNumber, email = item.ContactEmail });
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.PublicId == id);

            if (item == null)
            {
                return RedirectToAction("Error404", "Home");
            }

            // MISSING SECURITY FIX: Mask the number in the raw HTML so scrapers can't read it!
            item.ContactNumber = "Protected for Privacy";

            return View(item);
        }
    }
}