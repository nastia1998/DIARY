using DIARY_V4.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DIARY_V4
{
    /// <summary>
    /// Логика взаимодействия для ContactsWindow.xaml
    /// </summary>
    public partial class ContactsWindow : Window
    {
        public ContactsWindow()
        {
            InitializeComponent();
        }
        public string Login { get; set; }
        public int VRow { get; set; }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }

        BitmapImage image = new BitmapImage();
        private void ContactsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            bool p = IsUserAlreadyExist(VRow);
            if (p)
            {
                var dbContext = new BaseDbContext();
                var unitOfWork = new UnitOfWork(dbContext);

                var contact = unitOfWork.ContactRepository.Entities
                        .FirstOrDefault(n => n.Id_Contact == VRow);

                NameTextBox.Text = contact.Name;
                SurnameTextBox.Text = contact.Surname;
                DateOfBirthDP.Text = contact.DateOfBirth.Value.ToShortDateString();
                CountryTextBox.Text = contact.Country;
                CityTextBox.Text = contact.City;
                PhoneTextBox.Text = contact.Telephone;
                EmailTextBox.Text = contact.Email;                
                //image = new BitmapImage(new Uri(contact.Photo));
                Photo.Source = image;
            }
        }

        private void SaveContactbtn_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string pattern = @"[a-zA-Z1-9\-\._]+@[a-z1-9]+(.[a-z1-9]+){1,}";
                if (!Regex.IsMatch(EmailTextBox.Text, pattern))
                {
                    MessageBox.Show("Неправильно");
                }
                else
                {
                    bool k = IsUserAlreadyExist(VRow);
                    if (k)
                    {
                        var dbContext = new BaseDbContext();
                        var unitOfWork = new UnitOfWork(dbContext);

                        var contact = unitOfWork.ContactRepository.Entities
                                .FirstOrDefault(n => n.Id_Contact == VRow);

                        contact.Name = NameTextBox.Text;
                        contact.Surname = SurnameTextBox.Text;
                        contact.DateOfBirth = Convert.ToDateTime(DateOfBirthDP.Text);
                        contact.Country = CountryTextBox.Text;
                        contact.City = CityTextBox.Text;
                        contact.Telephone = PhoneTextBox.Text;
                        contact.Email = EmailTextBox.Text;
                        //contact.Photo = image.UriSource.LocalPath;
                        unitOfWork.Commit();
                        MessageBox.Show("Изменения прошли успешно");
                    }
                    else
                    {
                        var dbContext = new BaseDbContext();
                        var unitOfWork = new UnitOfWork(dbContext);

                        var user = unitOfWork.UserRepository.Entities
                                   .FirstOrDefault(n => n.Login == Login);

                        //if (NameTextBox.Text == "" || SurnameTextBox.Text == "" || CountryTextBox.Text == "" || CityTextBox.Text == ""
                        //    || PhoneTextBox.Text == "" || EmailTextBox.Text == "")
                        //{
                        //    MessageBox.Show("Одно из полей не заполнено. Вы желаете продолжить?", "Да", MessageBoxImage.)
                        //}
                        //var contact = new Contact()
                        //{
                        //    Name = NameTextBox.Text,
                        //    Surname = SurnameTextBox.Text,
                        //    DateOfBirth = Convert.ToDateTime(DateOfBirthDP.Text),
                        //    Country = CountryTextBox.Text,
                        //    City = CityTextBox.Text,
                        //    Telephone = PhoneTextBox.Text,
                        //    Email = EmailTextBox.Text,
                        //    Id_User = user.Id,
                        //    Photo = imgLocation
                        //};

                        var contact = new Contact();
                        contact.Name = NameTextBox.Text;
                        contact.Surname = SurnameTextBox.Text;
                        DateTime defaultDate = new DateTime();
                        DateTime birth = DateOfBirthDP.Text == "" ? defaultDate : Convert.ToDateTime(DateOfBirthDP.Text);
                        contact.DateOfBirth = birth;
                        contact.Country = CountryTextBox.Text;
                        contact.City = CityTextBox.Text;
                        contact.Telephone = PhoneTextBox.Text;
                        contact.Email = EmailTextBox.Text;
                        contact.Id_User = user.Id;
                        //contact.Photo = imgLocation;

                        unitOfWork.ContactRepository.Add(contact);
                        unitOfWork.Commit();
                        MessageBox.Show("Добавлен новый контакт");
                    }
                    this.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка добавления нового контакта\n" + ex.Message);
            }
        }

        private void DltContactbtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        string imgLocation = "";
        BitmapImage bitmap1;
        private void photoBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                bitmap1 = new BitmapImage(new Uri(op.FileName));
                Photo.Source = bitmap1;
                //Photo.Source = new BitmapImage(new Uri(op.FileName));
                imgLocation = op.FileName.ToString();
            }
        }

        private void leftBtn_Click(object sender, RoutedEventArgs e)
        {
            if (bitmap1 != null)
            {
                bitmap1.Rotation = Rotation.Rotate180;
                Photo.Source = bitmap1;
            }
        }

        private void rightBtn_Click(object sender, RoutedEventArgs e)
        {
            if (bitmap1 != null)
            {
                bitmap1.Rotation = Rotation.Rotate90;
                Photo.Source = bitmap1;
            }
        }

        private bool IsUserAlreadyExist(int id)
        {
            bool flag = false;

            var dbContext = new BaseDbContext();
            var unitOfWork = new UnitOfWork(dbContext);
            var contact = unitOfWork.ContactRepository.Entities
                    .FirstOrDefault(n => (n.User.Login == Login) && (n.Id_Contact == id));

            if (contact != null)
                return flag = true;
            return flag;
        }

        private void PhoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //string pattern = @"[0-9]";
            //if(Regex.IsMatch(PhoneTextBox.Text, pattern))
            //{
            //    MessageBox.Show("Правильно");
            //}
            //else
            //{
            //    MessageBox.Show("Неправильно");
            //}
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //string pattern = @"[a-zA-Z1-9\-\._]+@[a-z1-9]+(.[a-z1-9]+){1,}";
            //if(!Regex.IsMatch(EmailTextBox.Text, pattern))
            //{
            //    MessageBox.Show("Неправильно");
            //}
        }
    }
}
