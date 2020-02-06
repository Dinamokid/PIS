using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PisMirShow.Models
{
    public class WallPost
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public virtual User Author{ get; set; }

        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
