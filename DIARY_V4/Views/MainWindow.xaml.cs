using DIARY_V4.Model;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DIARY_V4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

       
        public string Login { get; set; }
        public new string Name { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NotVisible();
            OpenGrid.Visibility = Visibility.Visible;
            WelcomMessage();
        }

        private void WelcomMessage()
        {
            var dbContext = new BaseDbContext();
            var unitOfWork = new UnitOfWork(dbContext);

            var user = unitOfWork.UserRepository.Entities
                        .FirstOrDefault(b => b.Login == Login);

            DateTime curDate = DateTime.Today.Date;

            var reminders = unitOfWork.ReminderRepository.Entities
                            .Where(b => (b.Id_User == user.Id) && (b.Date >= curDate.Date))
                            .OrderBy(m => m.Date).Take(3).ToList();

            string res = "";
            int i = 1;
            foreach(var r in reminders)
            {   
                res += (i++ + ") " + r.Date.ToShortDateString() + " " + r.Time + " " + r.Text + "\n");
            }
            OpenWelcomTextBlock.Text = "Привет, " + user.Name + "\n" + "Ближайшие напоминания:\n";
            OpenRemindersTextBlock.Text = res;
        }

        #region Меню

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Заметка

        private void NewNote_Selected(object sender, RoutedEventArgs e)
        {
            NotVisible();
            NotesGrid.Visibility = Visibility.Visible;
        }

        private void AllNotesDG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UpdateNote();
        }

        private void SaveNoteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string rtbText = new TextRange(NoteRichTextBox.Document.ContentStart, NoteRichTextBox.Document.ContentEnd).Text; //string to save to db
                TextRange textRange = new TextRange(NoteRichTextBox.Document.ContentStart, NoteRichTextBox.Document.ContentEnd);
                var text = textRange.Text;

                if (text.Length < 4000)
                {
                    if (rtbText != "\r\n")
                    {
                        var dbContext = new BaseDbContext();
                        var unitOfWork = new UnitOfWork(dbContext);

                        var user = unitOfWork.UserRepository.Entities
                                    .FirstOrDefault(n => n.Login == Login);

                        string date = Convert.ToDateTime(DateNote.Text).ToString("yyyy-MM-dd");
                        DateTime selDate = Convert.ToDateTime(DateNote.Text);

                        var Oldnote = unitOfWork.NoteRepository.Entities
                                    .FirstOrDefault(b => (b.User.Login == Login) && (b.Date == selDate));


                        if (Oldnote != null)
                        {
                            Oldnote.Text = Oldnote.Text + "------------------------------------------------------------------------\n" + rtbText;
                            var res = MessageBox.Show("Заметка на данную дату уже была создана. Дополнить ее?", "Дополнение заметки", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (res == MessageBoxResult.Yes)
                            {
                                unitOfWork.Commit();
                            }
                        }
                        else
                        {
                            var note = new Note() { Date = Convert.ToDateTime(date), Id_User = user.Id, Text = rtbText };
                            unitOfWork.NoteRepository.Add(note);
                            unitOfWork.Commit();
                            for (int i = 0; i < a; i++)
                            {
                                var attPhoto = new AttPhoto { Path = Locations[i], Id_Note = note.Id_Note };
                                unitOfWork.PhotosRepository.Add(attPhoto);
                                unitOfWork.Commit();                                
                            }
                            a = 0; // чтобы можно было добавить фотографии к заметке на другой день сразу после добавления на текущий
                            Locations.Clear();
                            firstImg.Source = null;
                            secImg.Source = null;
                            thirdImg.Source = null;
                            firstImg.IsEnabled = true;
                            secImg.IsEnabled = true;
                            thirdImg.IsEnabled = true;
                            MessageBox.Show("Добавлена новая заметка");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не было введено текста");
                    }
                }
                else
                {
                    MessageBox.Show("Было введено слишком много символов!", "Превышение количества символов", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
               

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateNoteBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateNote();
        }

        private void UpdateNote()
        {
            try
            {
                var dbContext = new BaseDbContext();
                UnitOfWork unitOfWork = new UnitOfWork(dbContext);

                int row = AllNotesDG.SelectedIndex;
                if (row != -1)
                {
                    var ci = new DataGridCellInfo(AllNotesDG.Items[row], AllNotesDG.Columns[0]);
                    var crow = ci.Column.GetCellContent(ci.Item) as TextBlock;
                    var vrow = crow.Text;
                    string cvrow = Convert.ToDateTime(vrow).ToString("yyyy-MM-dd");

                    UpdateNoteWindow updateNoteWindow = new UpdateNoteWindow();
                    updateNoteWindow.Login = Login;
                    updateNoteWindow.VRow = cvrow;
                    updateNoteWindow.Owner = this;
                    updateNoteWindow.ShowDialog();
                    UpdateNotesDG();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteNoteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int row = AllNotesDG.SelectedIndex;

                if (row != -1)
                {
                    var res = MessageBox.Show("Вы уверены, что хотите удалить заметку?\nВосстановление невозможно", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                        var dbContext = new BaseDbContext();
                        UnitOfWork unitOfWork = new UnitOfWork(dbContext);

                        var ci = new DataGridCellInfo(AllNotesDG.Items[row], AllNotesDG.Columns[0]);
                        var crow = ci.Column.GetCellContent(ci.Item) as TextBlock;
                        var vrow = crow.Text;
                        string date = Convert.ToDateTime(vrow).ToString("yyyy-MM-dd");
                        DateTime Date = Convert.ToDateTime(date);

                        var note = unitOfWork.NoteRepository.Entities
                                .FirstOrDefault(p => p.Date == Date);
                        unitOfWork.NoteRepository.Remove(note);
                        unitOfWork.Commit();
                        UpdateNotesDG();
                    }
                }
                else
                {
                    MessageBox.Show("Для удаления необходимо выбрать запись");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AllNotes_Selected(object sender, RoutedEventArgs e)
        {
            NotVisible();

            AllNotesGrid.Visibility = Visibility.Visible;

            UpdateNotesDG();
            
        }        

        private void UpdateNotesDG()
        {
            var dbContext = new BaseDbContext();
            var unitOfWork = new UnitOfWork(dbContext);
           
            var notes = unitOfWork.NoteRepository.Entities
                        .Where(n => n.User.Login == Login).ToList();

            AllNotesDG.ItemsSource = notes;
        }

        #endregion

        #region Контакты

        private void Contacts_Selected(object sender, RoutedEventArgs e)
        {
            NotVisible();
            ContactsGrid.Visibility = Visibility.Visible;
            UpdateContactsDG();
        }

        private void AddContactBtn_Click(object sender, RoutedEventArgs e)
        {
            ContactsWindow contactsWindow = new ContactsWindow();
            contactsWindow.Login = Login;
            contactsWindow.Owner = this;
            contactsWindow.ShowDialog();
            UpdateContactsDG();
        }

        private void UpdContactBtn_Click(object sender, RoutedEventArgs e)
        {           
            UpdateContact();
        }

        private void ContactsDG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UpdateContact();
        }

        private void UpdateContact()
        {
            try
            {
                var dbContext = new BaseDbContext();
                UnitOfWork unitOfWork = new UnitOfWork(dbContext);

                int row = ContactsDG.SelectedIndex;

                if (row != -1)
                {
                    var ci = new DataGridCellInfo(ContactsDG.Items[row], ContactsDG.Columns[0]);
                    var crow = ci.Column.GetCellContent(ci.Item) as TextBlock;
                    var vrow = crow.Text;

                    ContactsWindow contactsWindow = new ContactsWindow();
                    contactsWindow.Login = Login;
                    contactsWindow.VRow = Convert.ToInt32(vrow);
                    contactsWindow.Owner = this;
                    contactsWindow.ShowDialog();
                    UpdateContactsDG();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DelContactBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int row = ContactsDG.SelectedIndex;

                if (row != -1)
                {
                    var res = MessageBox.Show("Вы уверены, что хотите удалить Контакт?\nВосстановление невозможно", "Удаление контакта", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                        var dbContext = new BaseDbContext();
                        UnitOfWork unitOfWork = new UnitOfWork(dbContext);

                        var ci = new DataGridCellInfo(ContactsDG.Items[row], ContactsDG.Columns[0]);
                        var crow = ci.Column.GetCellContent(ci.Item) as TextBlock;
                        int vrow = Convert.ToInt32(crow.Text);

                        var contact = unitOfWork.ContactRepository.Entities
                                .FirstOrDefault(p => p.Id_Contact == vrow);
                        unitOfWork.ContactRepository.Remove(contact);
                        unitOfWork.Commit();
                        UpdateContactsDG();
                    }
                }
                else
                {
                    MessageBox.Show("Для удаления необходимо выбрать запись");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateContactsDG()
        {
            var dbContext = new BaseDbContext();
            var unitOfWork = new UnitOfWork(dbContext);

            var contacts = unitOfWork.ContactRepository.Entities
                        .Where(n => n.User.Login == Login)             
                        .ToList();

            ContactsDG.ItemsSource = contacts;
            
        }

        #endregion

        #region Напоминания

        private void Reminders_Selected(object sender, RoutedEventArgs e)
        {
            NotVisible();
            RemindersGrid.Visibility = Visibility.Visible;
            UpdateRemindersDG();
        }

        private void RemindersDataGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateReminder();
        }

        private void AddReminderBtn_Click(object sender, RoutedEventArgs e)
        {
            ReminderWindow reminderWindow = new ReminderWindow();
            reminderWindow.Login = Login;
            reminderWindow.Owner = this;
            reminderWindow.ShowDialog();
            UpdateRemindersDG();
        }

        private void UpdReminderBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateReminder();
        }

        private void UpdateReminder()
        {
            try
            {
                var dbContext = new BaseDbContext();
                UnitOfWork unitOfWork = new UnitOfWork(dbContext);

                int row = RemindersDataGrid.SelectedIndex;

                if (row != -1)
                {
                    var ci = new DataGridCellInfo(RemindersDataGrid.Items[row], RemindersDataGrid.Columns[0]);
                    var crow = ci.Column.GetCellContent(ci.Item) as TextBlock;
                    var vrow = crow.Text;
                    string cvrow = Convert.ToDateTime(vrow).ToString("yyyy-MM-dd");

                    ReminderWindow reminderWindow = new ReminderWindow();
                    reminderWindow.Login = Login;
                    reminderWindow.VRow = cvrow;
                    reminderWindow.Owner = this;
                    reminderWindow.ShowDialog();
                    UpdateRemindersDG();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DelReminderBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dbContext = new BaseDbContext();
                UnitOfWork unitOfWork = new UnitOfWork(dbContext);

                int row = RemindersDataGrid.SelectedIndex;

                if (row != -1)
                {
                    var ci = new DataGridCellInfo(RemindersDataGrid.Items[row], RemindersDataGrid.Columns[0]);
                    var crow = ci.Column.GetCellContent(ci.Item) as TextBlock;
                    string vrow = crow.Text;
                    DateTime cvrow = Convert.ToDateTime(vrow);

                    var reminder = unitOfWork.ReminderRepository.Entities
                            .FirstOrDefault(p => p.Date == cvrow);
                    unitOfWork.ReminderRepository.Remove(reminder);
                    unitOfWork.Commit();
                    MessageBox.Show("Напоминание успешно удалено");
                    UpdateRemindersDG();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateRemindersDG()
        {
            var dbContext = new BaseDbContext();
            var unitOfWork = new UnitOfWork(dbContext);
            var reminders = unitOfWork.ReminderRepository.Entities
                        .Where(n => n.User.Login == Login)               
                        .ToList();

            RemindersDataGrid.ItemsSource = reminders;
        }

        #endregion


        private void NotVisible()
        {
            NotesGrid.Visibility = Visibility.Collapsed;
            AllNotesGrid.Visibility = Visibility.Collapsed;
            ContactsGrid.Visibility = Visibility.Collapsed;
            RemindersGrid.Visibility = Visibility.Collapsed;
            OpenGrid.Visibility = Visibility.Collapsed;
        }

        static int a = 0;
        string img1Location = "";
        string img2Location = "";
        string img3Location = "";
        List<string> Locations = new List<string>();
        BitmapImage bitmap1;
        private void addPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var dbContext = new BaseDbContext();
            var unitOfWork = new UnitOfWork(dbContext);

            var photos = unitOfWork.PhotosRepository.Entities
                    .Where(b => (b.Note.Date == DateNote.SelectedDate) && (b.Note.User.Login == Login)).ToList();
            if (photos.Count == 0)
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Title = "Select a picture";
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                  "Portable Network Graphic (*.png)|*.png";
                if (op.ShowDialog() == true)
                {
                    if (firstImg.IsEnabled == true)
                    {
                        bitmap1 = new BitmapImage(new Uri(op.FileName));                        
                        firstImg.Source = bitmap1;
                        a++;
                        img1Location = op.FileName.ToString();
                        Locations.Add(img1Location);
                        firstImg.IsEnabled = false;
                    }
                    else if (secImg.IsEnabled == true)
                    {
                        bitmap1 = new BitmapImage(new Uri(op.FileName));
                        secImg.Source = bitmap1;
                        a++;
                        img2Location = op.FileName.ToString();
                        Locations.Add(img2Location);
                        secImg.IsEnabled = false;
                    }
                    else if (thirdImg.IsEnabled == true)
                    {
                        bitmap1 = new BitmapImage(new Uri(op.FileName));
                        thirdImg.Source = bitmap1;
                        a++;
                        img3Location = op.FileName.ToString();
                        Locations.Add(img3Location);
                        thirdImg.IsEnabled = false;
                    }
                    else
                    {
                        MessageBox.Show("Можно добавить только 3 картинки");
                    }
                }
            }
            else
            {
                MessageBox.Show("Фотографии к данной заметке уже есть");
            }
        }
    }
}
