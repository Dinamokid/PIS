using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PisMirShow.Models
{
    public class FileItem
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
        public virtual TaskItem Task { get; set; }

		public int? CreatedUserId { get; set; }
		public virtual User CreatedUser { get; set; }

		public int ConfirmedUserId { get; set; }

		public DateTime ConfirmedDateTime { get; set; }

        public FileItem()
        {
            this.Confirmed = false;
        }
    }
}
