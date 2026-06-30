using Microsoft.EntityFrameworkCore;
using MyTCSCAN;
using MyTCSCAN.Services;

var builder = WebApplication.CreateBuilder(args);
n// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration
var conn = builder.Configuration.GetConnectionString("TCSCAN") 
           ?? "Server=.;Database=TCSCAN;Trusted_Connection=True;";

// DbContext
builder.Services.AddDbContext<TcscanContext>(options =>
    options.UseSqlServer(conn));

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
