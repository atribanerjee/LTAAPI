using LTADB.POCO;
using Microsoft.EntityFrameworkCore;

namespace LTADB
{
    public class LTADBContext : DbContext
    {
        public LTADBContext(DbContextOptions<LTADBContext> options): base(options)
        {
        }

        public DbSet<users> Users { get; set; }
    }
}
