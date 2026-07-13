using EjustRecoveryHub.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EjustRecoveryHub.Models
{
    [Table("IdItems")]
    public class IdItem : ItemModel
    {
        [Required]
        public string IdName { get; set; }

        [Required]
        public string IdNumber { get; set; }
    }
}