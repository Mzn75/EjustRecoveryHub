using EjustRecoveryHub.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EjustRecoveryHub.Models
{
    // Tells Entity Framework to make a separate relational table just for IDs
    [Table("IdItems")]
    public class IdItem : ItemModel
    {
        [Required]
        public string? IdName { get; set; }

        [Required]
        public string? IdNumber { get; set; }
    }
}