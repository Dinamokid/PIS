using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PisMirShow.Models
{
    public class TaskComments
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public int? TaskId { get; set; }
        public virtual TaskInSystem Task { get; set; }
        [Required]
        public int? UserId { get; set; }
        public virtual User User { get; set; }
    }
}