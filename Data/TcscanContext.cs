using Microsoft.EntityFrameworkCore;
using MyTCSCAN.Models;

namespace MyTCSCAN
{
    public class TcscanContext : DbContext
    {
        public TcscanContext(DbContextOptions<TcscanContext> options) : base(options) { }
n        public DbSet<CheckRecord> CheckRecords { get; set; }
        public DbSet<VehicleInfo> VehicleInfos { get; set; }
n        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
