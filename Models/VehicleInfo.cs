using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTCSCAN.Models
{
    [Table("VehicleInfo")]
    public class VehicleInfo
    {
        [Key]
        [Column("idVL")]
        public string idVL { get; set; }
n        [Column("PlateNo")]
        public string PlateNo { get; set; }
    }
}
