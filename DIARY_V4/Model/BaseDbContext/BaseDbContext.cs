using System.Data.Entity;


namespace DIARY_V4.Model
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext() : base("DefaultConnection") { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Reminder> Reminders { get; set; }
        public virtual DbSet<AttPhoto> Photos { get; set; }
    }
}
