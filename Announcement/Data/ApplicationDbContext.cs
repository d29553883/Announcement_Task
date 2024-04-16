using Announcement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Announcement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Bulletin> Bulletins { get; set; }
    }
}
