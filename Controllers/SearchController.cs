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
        // 1. Inject the database context into the controller
        private readonly ApplicationDbContext _context;
        // Constructor to initialize the controller with the database context
        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 2. Search page with optional query and category filter
        [HttpGet]
        public async Task<IActionResult> Index(string query, string categoryFilter = "all")
        {
            // 1. Base Query: Only search for items that are still "Active"
            var itemsQuery = _context.Items.Where(i => i.Status == "Active").AsQueryable();

            // 2. Text Search: Check if the user typed something into the search bar
            if (!string.IsNullOrEmpty(query))
            {
                itemsQuery = itemsQuery.Where(i => i.LocationFound.Contains(query));
            }

            // 3. Category Filter
            if (!string.IsNullOrEmpty(categoryFilter) && categoryFilter.ToLower() != "all")
            {
                var f = categoryFilter.ToLower();

                if (f == "device") itemsQuery = itemsQuery.OfType<DeviceItem>();
                else if (f == "id") itemsQuery = itemsQuery.OfType<IdItem>();
                else if (f == "wallet") itemsQuery = itemsQuery.OfType<WalletItem>();
                else if (f == "jewelry") itemsQuery = itemsQuery.OfType<JewelryItem>();
                else if (f == "notebook") itemsQuery = itemsQuery.OfType<NotebookItem>();
            }

            // 4. Sort by newest and fetch from the database async.
            var searchResults = await itemsQuery
                .OrderByDescending(i => i.DateReported)
                .ToListAsync();

            // 5. Pass the search text back to the view so the search bar doesn't clear out
            ViewBag.CurrentQuery = query;
            ViewBag.CurrentCategory = categoryFilter;

            // 6. Return the results to the view
            return View(searchResults);
        }

        // 3. Search Lost Items 
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
                    // Filter for ID items
                    var idQuery = query.OfType<IdItem>();
                    // Apply filters based on the searchData properties
                    if (!string.IsNullOrEmpty(searchData.IdNumber))
                    {
                        idQuery = idQuery.Where(item => item.IdNumber == searchData.IdNumber);
                    }
                    if (!string.IsNullOrEmpty(searchData.IdName))
                    {
                        idQuery = idQuery.Where(item => item.IdName.Contains(searchData.IdName));
                    }
                    // Cast back to the base ItemModel type for further processing
                    query = idQuery.Cast<ItemModel>();
                    break;

                case "device":
                    // Filter for Device items
                    var deviceQuery = query.OfType<DeviceItem>();
                    // Apply filters based on the searchData properties
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
                    // Filter for Wallet items
                    var walletQuery = query.OfType<WalletItem>();
                    // Apply filters based on the searchData properties
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
                    // Filter for Jewelry items
                    var jewelryQuery = query.OfType<JewelryItem>();
                    // Apply filters based on the searchData properties
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
                    // Filter for Notebook items
                    var notebookQuery = query.OfType<NotebookItem>();
                    // Apply filters based on the searchData properties
                    if (!string.IsNullOrEmpty(searchData.NotebookColor))
                    {
                        notebookQuery = notebookQuery.Where(item => item.NotebookColor.Contains(searchData.NotebookColor));
                    }
                    query = notebookQuery.Cast<ItemModel>();
                    break;
            }

            // 4. Execute and Return results async.
            var finalResults = await query
                .OrderByDescending(i => i.DateReported)
                .ToListAsync();

            // 5. Return the results to the SearchResults view
            return View("~/Views/Items/SearchResults.cshtml", finalResults);
        }
    }
}