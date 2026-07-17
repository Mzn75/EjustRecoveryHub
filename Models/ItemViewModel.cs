using System.ComponentModel.DataAnnotations;

namespace EjustRecoveryHub.Models
{
    public class ItemViewModel
    {
        // This model acts as a middleman that grabs data from Entity Framework and passes it to the controller, which then passes it to the view.

        // 1. Universal Fields
        public int Id { get; set; }
        public Guid PublicId { get; set; }
        [Required(ErrorMessage = "Please select a category.")]
        public string? Category { get; set; }

        [Required(ErrorMessage = "Please enter where you found this item.")]
        public string? LocationFound { get; set; }
        public string? ContactNumber { get; set; }
        public string? ContactEmail { get; set; }
        public string? ItemLocation { get; set; }
        public IFormFile? ItemPhoto { get; set; }
        public string? PhotoPath { get; set; }
        public string Status { get; set; } = "Active";

        // 2. Category Specific Fields
        public string? DeviceBrand { get; set; }
        public string? DeviceModel { get; set; }
        public string? DeviceDescription { get; set; }

        public string? IdName { get; set; }
        public string? IdNumber { get; set; }

        public string? WalletColor { get; set; }
        public string? WalletBrandOrMaterial { get; set; }
        public string? WalletDescription { get; set; }

        public string? JewelryMaterial { get; set; }
        public string? JewelryType { get; set; }
        public string? JewelryDescription { get; set; }

        public string? NotebookColor { get; set; }
        public string? NotebookDescription { get; set; }

    }
}