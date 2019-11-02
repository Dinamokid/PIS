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

		public int? ConfirmedUserId { get; set; }

		public DateTime? ConfirmedDateTime { get; set; }

        public DocumentType DocType { get; set; }

        public FileItem()
        {
            Confirmed = false;
        }

    }
    public enum DocumentType
    {
        [Display(Name = "Не определён")]
        NotDetermined,
        [Display(Name = "Положение")]
        Position,
        [Display(Name = "Устав")]
        Charter,
        [Display(Name = "Инструкция")]
        Instruction,
        [Display(Name = "Указ")]
        Decree,
        [Display(Name = "Приказ")]
        Order,
        [Display(Name = "Распоряжение")]
        Direction,
        [Display(Name = "Акт")]
        Act,
        [Display(Name = "Письмо")]
        Letter,
        [Display(Name = "Объяснительная")]
        Explanatory,
        [Display(Name = "Заявление")]
        Statement,
        [Display(Name = "Жалоба")]
        Complaint,
    }
}
