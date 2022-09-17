using MovieAdvisor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace MovieAdvisor.DataLayer
{
    public class MovieAdvisorDBContext : DbContext
    {
        public  DbSet<Movies> Movies { get; set; }
        public  DbSet<Users> Users { get; set; }
        public  DbSet<MovieRating> MovieRating { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            
        }
    }
}
