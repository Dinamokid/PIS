using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PisMirShow.Models;

namespace PisMirShow
{
    public class PisDbContext : DbContext
    {
        public DbSet<WallPost> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<FileInSystem> Files { get; set; }
        public DbSet<TaskInSystem> Tasks { get; set; }
        public DbSet<TaskComments> TaskComments { get; set; }
        
        public PisDbContext(DbContextOptions<PisDbContext> options)
            : base(options)
        {
        }

    }
}
