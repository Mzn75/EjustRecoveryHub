using EjustRecoveryHub.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EjustRecoveryHub.Models
{
    [Table("NotebookItems")]
    public class NotebookItem : ItemModel
    {
        [Required]
        public string? NotebookColor { get; set; }
        public string? NotebookDescription { get; set; }
    }
}