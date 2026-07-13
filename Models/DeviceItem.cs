using EjustRecoveryHub.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EjustRecoveryHub.Models
{
    // Tells EF to make a separate relational table just for devices
    [Table("DeviceItems")]
    public class DeviceItem : ItemModel
    {
        [Required(ErrorMessage = "Brand is required.")]
        public string DeviceBrand { get; set; }

        [Required]
        public string DeviceModel { get; set; }

        [Required]
        public string DeviceDescription { get; set; }
    }
}