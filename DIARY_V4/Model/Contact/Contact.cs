using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIARY_V4.Model
{
    public class Contact
    {
        [Key]
        public int Id_Contact { get; set; }

        public int Id_User { get; set; }
        public string Photo { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }        
        public DateTime? DateOfBirth { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        

        [ForeignKey("Id_User")]
        public virtual User User { get; set; }
    }
}
