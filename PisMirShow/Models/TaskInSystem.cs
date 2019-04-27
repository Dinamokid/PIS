using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PisMirShow.Models
{
    public class TaskInSystem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? FromUser { get; set; }
        public int? ToUser { get; set; }
        public DateTime? StartDate { get; set; }
        //public int? FromUserId { get; set; }
        //public virtual User FromUser { get; set; }
        //public int? ToUserId { get; set; }
        //public virtual User ToUser { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DeadLine { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        public ICollection<FileInSystem> Files { get; set; }
        public string FilesId { get; set; }
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
            Status = TaskStatus.NotStarted;
            StartDate = DateTime.UtcNow;
        }

        //public GetFiles()
        //{
        //    var task = DbContext.Tasks.First(t => t.Id == temp.Id);
        //    var filesIds = temp.FilesId.Trim().Split(',');
        //    task.Files = DbContext.Files.Where(f => filesIds.Any(t => t == f.Id.ToString())).ToList();
        //}
    }
}
