using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CachingWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CachingWebApi.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            
        }
        public DbSet<Drivers> Drivers {get; set;}   
    }
}