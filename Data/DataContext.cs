using Microsoft.EntityFrameworkCore;
using ProjetoG_LocaGames.Entities;

namespace ProjetoG_LocaGames.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public void LimparTabela<T>() where T : class
        {
            Set<T>().RemoveRange(Set<T>());
            SaveChanges();
        }

        public DbSet<User> Users { get; set; }  
        public DbSet<Game> Games { get; set; }
        public DbSet<RentedGame> RentedGames { get; set;}
    }
}