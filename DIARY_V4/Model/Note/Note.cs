using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIARY_V4.Model
{
    public class Note
    {
        [Key]
        public int Id_Note { get; set; }

        public int Id_User { get; set; }

        public DateTime Date { get; set; }

        public string Text { get; set; }

        [ForeignKey("Id_User")]
        public virtual User User { get; set; }

        public virtual ICollection<AttPhoto> Photos { get; set; }
    }
}
