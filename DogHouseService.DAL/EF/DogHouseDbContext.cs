using DogHouseService.DAL.DbModels;
using Microsoft.EntityFrameworkCore;

namespace DogHouseService.DAL.EF
{
    public sealed class DogHouseDbContext : DbContext
    {
        public DogHouseDbContext(DbContextOptions<DogHouseDbContext> options) 
            : base(options) 
        {
        }

        public DbSet<DogDbModel> Dogs { get; set; } = null!;
    }
}
