// Data/DataContext.cs
using Microsoft.EntityFrameworkCore;
using PetsManagementAPI.Models;

namespace PetsManagementAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<AdoptionRequest> AdoptionRequests { get; set; }
        public DbSet<Admin> Admins { get; set; }
    }
}