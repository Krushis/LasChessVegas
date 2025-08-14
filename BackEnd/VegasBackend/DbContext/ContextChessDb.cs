using Microsoft.EntityFrameworkCore;
using VegasBackend.ServerLogic;

namespace VegasBackend.DbContex
{
    public class ContextChessDb : DbContext
    {
        public DbSet<GameDb> Games { get; set; }
        public DbSet<Player> Players { get; set; }

        public ContextChessDb(DbContextOptions<ContextChessDb> options) : base(options) 
        {
            //
        }
    }
}
