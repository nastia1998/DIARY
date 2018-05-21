using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DIARY_V4.Model
{
    public class AttPhoto
    {
        [Key]
        public int Id_Photo { get; set; }

        public int Id_Note { get; set; }

        public string Path { get; set; }

        [ForeignKey("Id_Note")]
        public virtual Note Note { get; set; }
    }
}
