using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PisMirShow.Models
{
    public class FileInSystem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public byte[] File { get; set; }
        public bool Сonfirmed { get; set; }
        [Required]
        public string Type { get; set; }
        public string Name { get; set; }
        [Required]
        public int? TaskId { get; set; }
        public TaskInSystem Task { get; set; }

        public FileInSystem()
        {
            this.Сonfirmed = false;
        }
    }
}
