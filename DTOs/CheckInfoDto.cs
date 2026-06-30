using System;
nnamespace MyTCSCAN.DTOs
{
    public class CheckInfoDto
    {
        public string PlateNo { get; set; }
        public double? Weight { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }
        public string IDCardPhoto { get; set; }
        public string SignPhoto { get; set; }
        public string CheckNo { get; set; }
        public DateTime? CheckTime { get; set; }
        public string InspectorName { get; set; }
        public string Marks { get; set; }
        public string Remark { get; set; }
    }
}
