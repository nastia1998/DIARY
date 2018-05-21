using DIARY_V4.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace DIARY_V4
{
    /// <summary>
    /// Логика взаимодействия для ReminderWindow.xaml
    /// </summary>
    public partial class ReminderWindow : Window
    {
        public ReminderWindow()
        {
            InitializeComponent();
        }

        public string Login { get; set; }
        public string VRow { get; set; }


        private void ReminderWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime date = Convert.ToDateTime(VRow);
            bool flag = IsReminderAlreadyExist(date);
            if(flag)
            {
                var dbContext = new BaseDbContext();
                var unitOfWork = new UnitOfWork(dbContext);
                var reminder = unitOfWork.ReminderRepository.Entities
                            .FirstOrDefault(n => (n.User.Login == Login) && (n.Date == date));
                DateOfReminder.Text = reminder.Date.ToString();
                TimeTextBox.Text = reminder.Time;
                ReminderRichTextBox.AppendText(reminder.Text);
            }
        }

        private void SaveReminderbtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime date1 = Convert.ToDateTime(VRow);
                bool flag = IsReminderAlreadyExist(date1);
                if (flag)
                {
                    var dbContext = new BaseDbContext();
                    var unitOfWork = new UnitOfWork(dbContext);
                    var reminder = unitOfWork.ReminderRepository.Entities
                                .FirstOrDefault(n => (n.User.Login == Login) && (n.Date == date1));
                    //reminder.Date = Convert.ToDateTime(DateOfReminder.Text);
                    reminder.Time = TimeTextBox.Text;
                    string rtbText = new TextRange(ReminderRichTextBox.Document.ContentStart, ReminderRichTextBox.Document.ContentEnd).Text;
                    reminder.Text = rtbText;
                    unitOfWork.Commit();
                    MessageBox.Show("Изменения успешно сохранены");
                }
                else
                {
                    string date = Convert.ToDateTime(DateOfReminder.Text).ToString("yyyy-MM-dd");
                    var dbContext = new BaseDbContext();
                    var unitOfWork = new UnitOfWork(dbContext);

                    var user = unitOfWork.UserRepository.Entities
                              .FirstOrDefault(n => n.Login == Login);


                    string rtbText = new TextRange(ReminderRichTextBox.Document.ContentStart, ReminderRichTextBox.Document.ContentEnd).Text;

                    var reminder = new Reminder()
                    {
                        Date = Convert.ToDateTime(date),
                        Time = TimeTextBox.Text,
                        Text = rtbText,
                        Id_User = user.Id
                    };

                    unitOfWork.ReminderRepository.Add(reminder);
                    unitOfWork.Commit();
                    MessageBox.Show("Добавлено новое напоминание");
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка с добавлением нового напоминания:\n" + ex.Message);
            }
        }

        private void CancelSaveReminder_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool IsReminderAlreadyExist(DateTime date)
        {
            bool flag = false;

            var dbContext = new BaseDbContext();
            var unitOfWork = new UnitOfWork(dbContext);
            var reminder = unitOfWork.ReminderRepository.Entities
                    .FirstOrDefault(n => (n.User.Login == Login) && (n.Date == date));

            if (reminder != null)
                return flag = true;
            return flag;
        }

    }
}
