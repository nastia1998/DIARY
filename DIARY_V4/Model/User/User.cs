using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIARY_V4.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(25)]
        [Required]
        public string Name { get; set; }

        [MaxLength(25)]
        [Required]
        public string Login { get; set; }

        [MaxLength(25)]
        [Required]
        public string Password { get; set; }

        [MaxLength(25)]
        [Required]
        public string SecretWord { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual ICollection<Reminder> Reminders { get; set; }
    }
}
