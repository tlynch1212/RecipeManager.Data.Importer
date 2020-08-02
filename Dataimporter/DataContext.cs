﻿using Dataimporter.Models;
using Microsoft.EntityFrameworkCore;

namespace Dataimporter
{
    public class DataContext : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Instruction> Instructions { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Database=recipemanager;Server=db-recipemanager.postgres.database.azure.com;User Id=tlynch1212@db-recipemanager;Password=6Q@juFTZFQy47E!O5Z@Zfc%c89$;SslMode=Require;");
        }
    }
}
