using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PisMirShow.Models
{
    public class FileInSystem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public byte[] File { get; set; }
        public bool Confirmed { get; set; }
        [Required]
        public string Type { get; set; }
        public string Name { get; set; }

        public int? TaskId { get; set; }
        public TaskInSystem Task { get; set; }

		public int? UserId { get; set; }
		public User User { get; set; }

		public int ConfirmedUserId { get; set; }

		public DateTime ConfirmedDateTime { get; set; }

        public FileInSystem()
        {
            this.Confirmed = false;
        }
    }
}
