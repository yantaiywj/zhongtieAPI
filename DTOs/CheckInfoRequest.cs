using System;
nnamespace MyTCSCAN.DTOs
{
    public class CheckInfoRequest
    {
        public string secretKey { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public string PlateNo { get; set; }
    }
}
