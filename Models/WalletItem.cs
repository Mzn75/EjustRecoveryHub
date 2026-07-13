using EjustRecoveryHub.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EjustRecoveryHub.Models
{
    // Tells EF to make a separate relational table just for wallets
    [Table("WalletItems")]
    public class WalletItem : ItemModel
    {
        [Required]
        public string? WalletColor { get; set; }
        [Required]
        public string? WalletBrandOrMaterial { get; set; }
        public string? WalletDescription { get; set; }
    }
}