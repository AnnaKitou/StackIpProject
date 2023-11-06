using IpStackAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace IpStackAPI.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<DetailsOfIp> DetailsOfIp { get; set; }
    }
}
