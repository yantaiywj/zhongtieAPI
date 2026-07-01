using System;
using System.Text.Json;

namespace MyTCSCAN.DTOs
{
    public class GatewayRequest
    {
        public GatewayHeader Header { get; set; }
        public JsonElement Body { get; set; }
    }

    public class GatewayHeader
    {
        public string secretKey { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public string PlateNo { get; set; }
        public string locatePort { get; set; }
    }
}
