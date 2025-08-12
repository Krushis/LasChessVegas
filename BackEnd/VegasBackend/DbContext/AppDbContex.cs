using Microsoft.EntityFrameworkCore;
using VegasBackend.ServerLogic;

namespace VegasBackend.DbContex
{
    public class AppDbContex : DbContext
    {
        public DbSet<GameDb> Games { get; set; }
        public DbSet<Player> Players { get; set; }

        public AppDbContex(DbContextOptions<AppDbContex> options) : base(options) 
        {
            //
        }
    }
}
