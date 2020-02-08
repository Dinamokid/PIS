using Microsoft.EntityFrameworkCore;
using PisMirShow.Models;

namespace PisMirShow
{
    public class PisDbContext : DbContext
    {
        public DbSet<WallPost> Posts { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<FileItem> Files { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<TaskComments> TaskComments { get; set; }
        public DbSet<Role> Roles { get; set; }
		public DbSet<DirectoryData> DirectoryData { get; set; }
        
        public PisDbContext(DbContextOptions<PisDbContext> options)
            : base(options)
        {
		}
	}
}
