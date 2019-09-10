using DIARY_V4.Model;
using System;
using System.Data.Entity;
using System.Linq;

namespace DIARY_V4.Model
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly BaseDbContext _dbContext;

        #region Repositories

        public IRepository<User> UserRepository => new GenericRepository<User>(_dbContext);
        public IRepository<Note> NoteRepository => new GenericRepository<Note>(_dbContext);
        public IRepository<Contact> ContactRepository => new GenericRepository<Contact>(_dbContext);
        public IRepository<Reminder> ReminderRepository => new GenericRepository<Reminder>(_dbContext);
        public IRepository<AttPhoto> PhotosRepository => new GenericRepository<AttPhoto>(_dbContext);

        #endregion

        public UnitOfWork(BaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

    }
}
