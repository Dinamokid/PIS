using PisMirShow.Models.Account;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PisMirShow.Models.Dialogs
{
    public class Message
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public virtual User Author { get; set; }

        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }

        public bool isReaded { get; set; }

        public int DialogId { get; set; }
        public virtual Dialog Dialog { get; set; }

        [NotMapped]
        public int TotalCount {get; set;}
	}
}