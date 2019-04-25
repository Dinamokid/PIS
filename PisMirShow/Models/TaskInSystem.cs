using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PisMirShow.Models
{
    public class TaskInSystem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int FromUser { get; set; }
        public int ToUser { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DeadLine { get; set; }
        public string Text { get; set; }
        public ICollection<FileInSystem> Files { get; set; }
        public TaskStatus Status { get; set; }

        public enum TaskStatus
        {
            NotStarted,
            Active,
            Paused,
            Сonfirmed, //подтверждена
            Finished,
            Expited //просрочена
        }

        public TaskInSystem()
        {
            Files = new List<FileInSystem>();
        }
    }
}
