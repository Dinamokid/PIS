using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PisMirShow.Models
{
    public class TaskItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime? StartDate { get; set; }
        public int? FromUserId { get; set; }
        public virtual User FromUser { get; set; }
        public int? ToUserId { get; set; }
        public virtual User ToUser { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DeadLine { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        public virtual ICollection<FileItem> Files { get; set; }
        public string FilesId { get; set; }
        public TaskStatus Status { get; set; }
        public virtual ICollection<TaskComments> Comments { get; set; }

        public enum TaskStatus
        {
            [Display(Name = "Не активно")]
            NotStarted,
            [Display(Name = "Активно")]
            Active,
            [Display(Name = "Остановлено")]
            Paused,
            [Display(Name = "Подтверждено")]
            Сonfirmed, 
            [Display(Name = "Завершено")]
            Finished,
        }

        public TaskItem()
        {
            Status = TaskStatus.NotStarted;
            StartDate = DateTime.UtcNow;
        }
    }
}
