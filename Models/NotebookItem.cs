using EjustLostAndFoundHub.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EjustLostAndFoundHub.Models
{
    // Tells Entity Framework to make a separate relational table just for notebooks
    [Table("NotebookItems")]
    public class NotebookItem : ItemModel
    {
        [Required]
        public string? NotebookColor { get; set; }
        public string? NotebookDescription { get; set; }
    }
}