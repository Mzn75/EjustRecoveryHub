using EjustRecoveryHub.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EjustRecoveryHub.Models
{
    // Tells Entity Framework to make a separate relational table just for jewelrys
    [Table("JewelryItems")]
    public class JewelryItem : ItemModel
    {
        [Required]
        public string? JewelryType { get; set; }
        [Required]
        public string? JewelryMaterial { get; set; }
        public string? JewelryDescription { get; set; }
    }
}