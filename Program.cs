using Microsoft.EntityFrameworkCore;
using MyTCSCAN;
using MyTCSCAN.Services;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration
var conn = builder.Configuration.GetConnectionString("TCSCAN") 
           ?? "Server=.;Database=TCSCAN;Trusted_Connection=True;";

// DbContext
builder.Services.AddDbContext<TcscanContext>(options =>
    options.UseSqlServer(conn));

// HttpClient for Ticket system with Polly policies (retry + circuit breaker)
builder.Services.AddHttpClient("TicketClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AppSettings:TicketBaseUrl"] ?? "https://ticket.system/");
    client.Timeout = TimeSpan.FromSeconds(20);
})
.AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
    .OrResult(msg => (int)msg.StatusCode == 429)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
.AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

// App services
builder.Services.AddScoped<IImageService, ImageService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();
