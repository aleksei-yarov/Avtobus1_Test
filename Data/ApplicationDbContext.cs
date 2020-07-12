using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test_Bus.Models;

namespace Test_Bus.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Link> LinksAvtobus { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        
    }
}
