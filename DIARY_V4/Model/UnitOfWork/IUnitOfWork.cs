using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIARY_V4.Model
{
    public interface IUnitOfWork
    {
        IRepository<User> UserRepository { get; }
        IRepository<Note> NoteRepository { get; }
        IRepository<Contact> ContactRepository { get; }
        IRepository<Reminder> ReminderRepository { get; }
        IRepository<AttPhoto> PhotosRepository { get; }

        /// <summary>
        /// Commit all changes
        /// </summary>
        void Commit();

        void Dispose();
    }
}
