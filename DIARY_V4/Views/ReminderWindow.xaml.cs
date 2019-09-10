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
            if (flag)
            {
                DateOfReminder.IsEnabled = false;
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
                DateTime? date1 = DateOfReminder.SelectedDate;
                bool flag = IsReminderAlreadyExist(date1);
                string rtbText1 = new TextRange(ReminderRichTextBox.Document.ContentStart, ReminderRichTextBox.Document.ContentEnd).Text; //string to save to db
                if (TimeTextBox.Text != "" && rtbText1 != "")
                {
                    if (DateOfReminder.SelectedDate != null)
                    {
                        if (rtbText1.Length < 200)
                        {
                            if (flag)
                            {
                                var dbContext = new BaseDbContext();
                                var unitOfWork = new UnitOfWork(dbContext);
                                var reminder = unitOfWork.ReminderRepository.Entities
                                            .FirstOrDefault(n => (n.User.Login == Login) && (n.Date == date1));

                                if (reminder != null)
                                {
                                    if (DateOfReminder.IsEnabled == false) //если окно открыто для изменений
                                    {
                                        reminder.Text = rtbText1;
                                        reminder.Time = TimeTextBox.Text;
                                        unitOfWork.Commit();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Напоминание на данный день уже было записано.\n Для его изменения перейдите в напоминание");
                                    }
                                }
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
                        }
                        else
                        {
                            MessageBox.Show("Превышено максильное количество символов в тексте напоминания");
                        }

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Необходимо ввести дату напоминания");
                    }
                }
                else
                {
                    MessageBox.Show("Необходимо указать время и текст напоминания!");
                }
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

        private bool IsReminderAlreadyExist(DateTime? date)
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
