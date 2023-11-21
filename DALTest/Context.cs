using DALTest.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestServices;

namespace DALTest
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Entities.Group> Groups { get; set; }
        public DbSet<Entities.Test> Tests { get; set; }
        public DbSet<Entities.Question> Questions { get; set; }
        public DbSet<Entities.Answer> Answers { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<UserTest> UserTests { get; set; }
        public Context(DbContextOptions<Context> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            DbSeeder.SeedData(modelBuilder);
        }

    }
}
