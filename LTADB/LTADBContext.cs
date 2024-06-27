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
        public DbSet<Scenario1> Scenario1 { get; set; }
        public DbSet<scoreboard> Scoreboard { get; set; }
        public DbSet<scenarioaster> Scenarioaster { get; set; }

    }
}
