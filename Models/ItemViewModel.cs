using System.ComponentModel.DataAnnotations;

namespace EjustRecoveryHub.Models
{
    public class ItemViewModel
    {
        // --- Required Form Inputs ---
        public int Id { get; set; }
        [Required(ErrorMessage = "Please select a category.")]
        public string? Category { get; set; }

        [Required(ErrorMessage = "Please enter where you found this item.")]
        public string? LocationFound { get; set; }

        // --- Optional Form Inputs ---
        public string? ContactNumber { get; set; }
        public string? ItemLocation { get; set; }
        public IFormFile? ItemPhoto { get; set; }
        public string? PhotoPath { get; set; }
        public string Status { get; set; } = "Active";

        // --- Category Specific Fields ---
        public string? DeviceBrand { get; set; }
        public string? DeviceModel { get; set; }
        public string? DeviceDescription { get; set; }

        public string? IdName { get; set; }
        public string? IdNumber { get; set; }

        public string? WalletColor { get; set; }
        public string? WalletBrandOrMaterial { get; set; }

        public string? JewelryMaterial { get; set; }
        public string? JewelryType { get; set; }

        public string? NotebookColor { get; set; }
    }
}