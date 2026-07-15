using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EjustRecoveryHub.Data;
using EjustRecoveryHub.Models;
using static EjustRecoveryHub.Models.ItemViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace EjustRecoveryHub.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string query, string categoryFilter = "all")
        {
            // 1. BASE QUERY: Only search for items that are still "Active"
            var itemsQuery = _context.Items.Where(i => i.Status == "Active").AsQueryable();

            // 2. TEXT SEARCH: Check if the user typed something into the search bar
            if (!string.IsNullOrEmpty(query))
            {
                // Removed .ToLower() for better SQL performance
                itemsQuery = itemsQuery.Where(i => i.LocationFound.Contains(query));
            }

            // 3. CATEGORY FILTER: Use polymorphic type filtering instead of strings
            if (!string.IsNullOrEmpty(categoryFilter) && categoryFilter.ToLower() != "all")
            {
                var f = categoryFilter.ToLower();

                if (f == "device") itemsQuery = itemsQuery.OfType<DeviceItem>();
                else if (f == "id") itemsQuery = itemsQuery.OfType<IdItem>();
                else if (f == "wallet") itemsQuery = itemsQuery.OfType<WalletItem>();
                else if (f == "jewelry") itemsQuery = itemsQuery.OfType<JewelryItem>();
                else if (f == "notebook") itemsQuery = itemsQuery.OfType<NotebookItem>();
            }

            // 4. EXECUTE ASYNC: Sort by newest and fetch from the database
            var searchResults = await itemsQuery
                .OrderByDescending(i => i.DateReported)
                .ToListAsync();

            // Pass the search text back to the view so the search bar doesn't clear out
            ViewBag.CurrentQuery = query;
            ViewBag.CurrentCategory = categoryFilter;

            // FIX: Actually hand the results to the HTML view!
            return View(searchResults);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(ItemViewModel searchData)
        {
            // 1. Validate Category
            if (string.IsNullOrEmpty(searchData.Category))
            {
                return RedirectToAction("LostItems", "Items");
            }

            // 2. Start the SQL Query (Only Active items)
            IQueryable<ItemModel> query = _context.Items.Where(i => i.Status == "Active");

            // 3. Advanced Relational Filtering
            switch (searchData.Category.ToLower())
            {
                case "id":

                    // Filter down into the specific IdItems table
                    var idQuery = query.OfType<IdItem>();

                    if (!string.IsNullOrEmpty(searchData.IdNumber))
                    {
                        idQuery = idQuery.Where(item => item.IdNumber == searchData.IdNumber);
                    }
                    if (!string.IsNullOrEmpty(searchData.IdName))
                    {
                        idQuery = idQuery.Where(item => item.IdName.Contains(searchData.IdName));
                    }
                    // Cast back to the base query so we can execute it later
                    query = idQuery.Cast<ItemModel>();
                    break;

                case "device":
                    var deviceQuery = query.OfType<DeviceItem>();

                    if (!string.IsNullOrEmpty(searchData.DeviceBrand))
                    {
                        deviceQuery = deviceQuery.Where(item => item.DeviceBrand.Contains(searchData.DeviceBrand));
                    }
                    if (!string.IsNullOrEmpty(searchData.DeviceModel))
                    {
                        deviceQuery = deviceQuery.Where(item => item.DeviceModel.Contains(searchData.DeviceModel));
                    }
                    if (!string.IsNullOrEmpty(searchData.DeviceDescription))
                    {
                        deviceQuery = deviceQuery.Where(item => item.DeviceDescription.Contains(searchData.DeviceDescription));
                    }
                    query = deviceQuery.Cast<ItemModel>();
                    break;

                case "wallet":
                    var walletQuery = query.OfType<WalletItem>();

                    if (!string.IsNullOrEmpty(searchData.WalletColor))
                    {
                        walletQuery = walletQuery.Where(item => item.WalletColor.Contains(searchData.WalletColor));
                    }
                    if (!string.IsNullOrEmpty(searchData.WalletBrandOrMaterial))
                    {
                        walletQuery = walletQuery.Where(item => item.WalletBrandOrMaterial.Contains(searchData.WalletBrandOrMaterial));
                    }
                    query = walletQuery.Cast<ItemModel>();
                    break;

                case "jewelry":
                    var jewelryQuery = query.OfType<JewelryItem>();

                    if (!string.IsNullOrEmpty(searchData.JewelryType))
                    {
                        jewelryQuery = jewelryQuery.Where(item => item.JewelryType.Contains(searchData.JewelryType));
                    }
                    if (!string.IsNullOrEmpty(searchData.JewelryMaterial))
                    {
                        jewelryQuery = jewelryQuery.Where(item => item.JewelryMaterial.Contains(searchData.JewelryMaterial));
                    }
                    query = jewelryQuery.Cast<ItemModel>();
                    break;

                case "notebook":
                    var notebookQuery = query.OfType<NotebookItem>();

                    if (!string.IsNullOrEmpty(searchData.NotebookColor))
                    {
                        notebookQuery = notebookQuery.Where(item => item.NotebookColor.Contains(searchData.NotebookColor));
                    }
                    query = notebookQuery.Cast<ItemModel>();
                    break;
            }

            // 4. Execute and Return ASYNCHRONOUSLY 
            var finalResults = await query
                .OrderByDescending(i => i.DateReported)
                .ToListAsync();

            return View("~/Views/Items/SearchResults.cshtml", finalResults);
        }
    }
}