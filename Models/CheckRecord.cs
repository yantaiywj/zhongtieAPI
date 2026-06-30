using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTCSCAN.Models
{
    [Table("CheckRecord")]
    public partial class CheckRecord
    {
        public CheckRecord()
        {
            IfDanger = false;
        }
n        [Key]
        [Column("SN")]
        [Required]
        public string SN { get; set; }
n        [Column("idDL")]
        public string idDL { get; set; }
n        [Column("idVL")]
        public string idVL { get; set; }

        [Column("IDChkIn")]
        public string IDChkIn { get; set; }
n        [Column("DTChkIn")]
        public DateTime? DTChkIn { get; set; }

        [Column("idManner")]
        public int? idManner { get; set; }

        [Column("IDChk")]
        public string IDChk { get; set; }

        [Column("DTChk")]
        public DateTime? DTChk { get; set; }

        [Column("NTChk")]
        public string NTChk { get; set; }

        [Column("idResChk")]
        public int? idResChk { get; set; }

        [Column("IDReview")]
        public string IDReview { get; set; }

        [Column("idResRev")]
        public int? idResRev { get; set; }

        [Column("DTReview")]
        public DateTime? DTReview { get; set; }

        [Column("NTRev")]
        public string NTRev { get; set; }

        [Column("L")]
        public int? L { get; set; }

        [Column("W")]
        public int? W { get; set; }

        [Column("H")]
        public int? H { get; set; }

        [Column("Weight")]
        public int? Weight { get; set; }

        [Column("Goods")]
        public string Goods { get; set; }

        [Column("IfDanger")]
        [Required]
        public bool IfDanger { get; set; }

        [Column("shippingorder")]
        public string shippingorder { get; set; }

        [Column("HostName")]
        public string HostName { get; set; }

        [Column("RoadNo")]
        public int? RoadNo { get; set; }

        [Column("RoadName")]
        public string RoadName { get; set; }

        [Column("UVSPath")]
        public string UVSPath { get; set; }

        [Column("AreaId")]
        public string AreaId { get; set; }

        [NotMapped]
        public int? idvt { get; set; }

        [NotMapped]
        public int? typecode { get; set; }

        [NotMapped]
        public string ImgFile { get; set; }

        [NotMapped]
        public string Tel { get; set; }
    }
}
