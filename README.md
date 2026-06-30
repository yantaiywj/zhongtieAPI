# TCSCAN WebAPI - CheckInfo 服务示例

说明
- 提供 POST /Data/CheckInfo 接口，接收 JSON 请求并返回查验记录（符合你给定的返回格式）。
- 需要配置数据库连接字符串、secretKey、图片根路径等。

运行（示例）
1. 安装 .NET 7 SDK（或 .NET 6，代码基于 minimal hosting model）。
2. 更新 appsettings.json 中的 ConnectionStrings 和 AppSettings（SecretKey、ImageBasePath 等）。
3. 运行 `dotnet run`。
4. 使用 Postman 发送 POST 请求到 http://localhost:5000/Data/CheckInfo

示例请求 body:
{
  "secretKey":"5053312470ca328281d659cac0f499ad",
  "BeginTime":"2025-12-18T00:00:00Z",
  "EndTime":"2025-12-18T24:00:00Z",
  "PlateNo":"ABC123"
}

返回格式（示例）:
{
  "code":"1",
  "msg":"查询成功",
  "data":[ { ... } ],
  "success": true
}

注意
- 将图片路径规则（UVSPath、HostName 等）告知我或修改 ImageService.GetImageFullPath 实现，以便找到图片并转换为 base64。
- 根据真实表结构调整 VehicleInfo/CheckRecord 的字段映射与关联。
