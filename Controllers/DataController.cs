using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyTCSCAN.DTOs;
using MyTCSCAN.Models;
using MyTCSCAN.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTCSCAN.Controllers
{
    [ApiController]
    [Route("Data")]
    public class DataController : ControllerBase
    {
        private readonly TcscanContext _db;
        private readonly IConfiguration _config;
        private readonly IImageService _imgService;_n        
        private readonly string _secretKey;
        private readonly string[] _dangerKeywords;
        private readonly int _maxReturnRows;

        public DataController(TcscanContext db, IConfiguration config, IImageService imgService)
        {
            _db = db;
            _config = config;
            _imgService = imgService;
            _secretKey = _config["AppSettings:SecretKey"];
            _dangerKeywords = _config.GetSection("AppSettings:DangerKeywords").Get<string[]>() ?? new string[0];
            _maxReturnRows = int.TryParse(_config["AppSettings:MaxReturnRows"], out var n) ? n : 200;
        }

        [HttpPost("CheckInfo")]
        public async Task<IActionResult> CheckInfo([FromBody] CheckInfoRequest req)
        {
            if (req == null)
                return BadRequest(new { code = "400", msg = "请求体为空", data = new object[0], success = false });

            if (string.IsNullOrWhiteSpace(req.secretKey) || req.secretKey.Trim() != _secretKey)
                return Unauthorized(new { code = "401", msg = "secretKey invalid", data = new object[0], success = false });

            if (req.BeginTime > req.EndTime)
                return BadRequest(new { code = "400", msg = "BeginTime > EndTime", data = new object[0], success = false });

            var query = from c in _db.CheckRecords
                        join v in _db.VehicleInfos on c.idVL equals v.idVL into gj
                        from v in gj.DefaultIfEmpty()
                        where ((c.DTChk >= req.BeginTime && c.DTChk <= req.EndTime)
                                || (c.DTReview >= req.BeginTime && c.DTReview <= req.EndTime)
                                || (c.DTChkIn >= req.BeginTime && c.DTChkIn <= req.EndTime))
                        select new { c, PlateNo = v != null ? v.PlateNo : null };

            if (!string.IsNullOrEmpty(req.PlateNo))
                query = query.Where(x => x.PlateNo == req.PlateNo);

            var list = await query.OrderByDescending(x => x.c.DTChk ?? x.c.DTReview ?? x.c.DTChkIn)
                                  .Take(_maxReturnRows)
                                  .ToListAsync();

            if (list == null || list.Count == 0)
                return Ok(new { code = "0", msg = "无对应数据", data = new object[0], success = true });

            var result = new List<CheckInfoDto>();
            foreach (var item in list)
            {
                var r = item.c;
                var dto = new CheckInfoDto
                {
                    PlateNo = item.PlateNo,
                    Weight = r.Weight.HasValue ? (double?)r.Weight.Value : null,
                    Length = r.L.HasValue ? (double?)r.L.Value : null,
                    Width = r.W.HasValue ? (double?)r.W.Value : null,
                    Height = r.H.HasValue ? (double?)r.H.Value : null,
                    CheckNo = r.SN,
                    CheckTime = r.DTChk ?? r.DTReview ?? r.DTChkIn,
                    InspectorName = r.IDChk,
                    Remark = r.NTChk
                };

                var marks = new List<string>();
                if (!string.IsNullOrWhiteSpace(r.Goods))
                {
                    foreach (var k in _dangerKeywords)
                    {
                        if (r.Goods.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            marks.Add(k);
                        }
                    }
                }
                dto.Marks = marks.Count > 0 ? string.Join(",", marks) : "";

                string idCardBase64 = _imgService.GetBase64Image(r.UVSPath);
                dto.IDCardPhoto = idCardBase64;

                string signBase64 = null;
                if (!string.IsNullOrEmpty(r.ImgFile))
                {
                    signBase64 = _imgService.GetBase64Image(r.ImgFile);
                }
                dto.SignPhoto = signBase64;

                result.Add(dto);
            }

            return Ok(new { code = "1", msg = "查询成功", data = result, success = true });
        }
    }
}
