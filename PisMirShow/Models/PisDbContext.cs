using Microsoft.EntityFrameworkCore;
using PisMirShow.Models;
using PisMirShow.Models.Account;
using PisMirShow.Models.Dialogs;

namespace PisMirShow
{
    public class PisDbContext : DbContext
    {
        public DbSet<WallPost> Posts { get; set; }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Dialog> Dialogs { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<FileItem> Files { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<TaskComments> TaskComments { get; set; }
        public DbSet<Role> Roles { get; set; }
		public DbSet<DirectoryData> DirectoryData { get; set; }

        //ManyToMany
        public DbSet<UserDialog> UsersDialogs { get; set; } 

        public PisDbContext(DbContextOptions<PisDbContext> options)
            : base(options)
        {
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ForNpgsqlUseIdentityColumns();
			//ManyToMany UserDialogs
			modelBuilder.Entity<UserDialog>()
				.HasKey(ud => new { ud.UserId, ud.DialogId });
			modelBuilder.Entity<UserDialog>()
				.HasOne(bc => bc.User)
				.WithMany(b => b.Dialogs)
				.HasForeignKey(bc => bc.UserId);
			modelBuilder.Entity<UserDialog>()
				.HasOne(bc => bc.Dialog)
				.WithMany(c => c.Users)
				.HasForeignKey(bc => bc.DialogId);
		}
	}
}
