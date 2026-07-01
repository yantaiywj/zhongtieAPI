using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MyTCSCAN.Controllers
{
    [ApiController]
    [Route("api/gateway")]
    public class GatewayController : ControllerBase
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly ILogger<GatewayController> _logger;
        private readonly string _secretKey;

        public GatewayController(IHttpClientFactory httpFactory, IConfiguration config, ILogger<GatewayController> logger)
        {
            _httpFactory = httpFactory;
            _logger = logger;
            _secretKey = config["AppSettings:SecretKey"];
        }

        [HttpPost("vehicleinfo")]
        public async Task<IActionResult> VehicleInfo([FromBody] DTOs.GatewayRequest req)
        {
            var traceId = Request.Headers.ContainsKey("X-Trace-Id") ? Request.Headers["X-Trace-Id"].ToString() : Guid.NewGuid().ToString();
            using (_logger.BeginScope(new Dictionary<string, object> { { "TraceId", traceId } }))
            {
                _logger.LogInformation("Received VehicleInfo request Plate={Plate} Begin={Begin} End={End}", req?.Header?.PlateNo, req?.Header?.BeginTime, req?.Header?.EndTime);

                if (req == null || req.Header == null)
                    return BadRequest(new { code = "400", msg = "invalid payload", success = false });

                if (string.IsNullOrWhiteSpace(req.Header.secretKey) || req.Header.secretKey.Trim() != _secretKey)
                    return Unauthorized(new { code = "401", msg = "secretKey invalid", success = false });

                // Build forward payload to ticket system
                var forward = new
                {
                    PlateNo = req.Header.PlateNo,
                    BeginTime = req.Header.BeginTime,
                    EndTime = req.Header.EndTime,
                    locatePort = req.Header.locatePort
                };

                var client = _httpFactory.CreateClient("TicketClient");
                // The relative URL here can be adjusted via configuration if needed
                var ticketRelativePath = "api/vehicleinfo"; 
                var reqMsg = new HttpRequestMessage(HttpMethod.Post, ticketRelativePath);
                reqMsg.Headers.Add("X-Trace-Id", traceId);
                var json = JsonSerializer.Serialize(forward);
                reqMsg.Content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var resp = await client.SendAsync(reqMsg);
                    var respStr = await resp.Content.ReadAsStringAsync();
                    _logger.LogInformation("Forwarded to ticket system. Status:{Status} Body:{Body}", resp.StatusCode, respStr);

                    return new ContentResult
                    {
                        Content = respStr,
                        ContentType = "application/json",
                        StatusCode = (int)resp.StatusCode
                    };
                }
                catch (Polly.CircuitBreaker.BrokenCircuitException ex)
                {
                    _logger.LogWarning(ex, "Circuit is open when calling ticket system");
                    return StatusCode(503, new { code = "503", msg = "Ticket system unavailable (circuit open)", success = false });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error forwarding to ticket system");
                    // Optionally enqueue request for later retry
                    return StatusCode(502, new { code = "502", msg = "Forward error", success = false });
                }
            }
        }
    }
}
