using DIARY_V4.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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
                if (contact.DateOfBirth != null)
                {
                    DateOfBirthDP.Text = contact.DateOfBirth.Value.ToShortDateString();
                }
                CountryTextBox.Text = contact.Country;
                CityTextBox.Text = contact.City;
                PhoneTextBox.Text = contact.Telephone;
                EmailTextBox.Text = contact.Email;
                try
                {
                    if (contact.Photo == "") //если фотографии нет
                    {
                        contact.Photo = "/Images/noimagefound.jpg";
                        unitOfWork.Commit();
                        Uri uri = new Uri(contact.Photo, UriKind.Relative);
                        image = new BitmapImage(uri);
                        Photo.Source = image;
                    }
                    else if (contact.Photo == "/Images/noimagefound.jpg")
                    {
                        try
                        {
                            Uri uri = new Uri(contact.Photo, UriKind.Relative);
                            image = new BitmapImage(uri);
                            Photo.Source = image;
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else
                    {
                        image = new BitmapImage(new Uri(contact.Photo));
                        Photo.Source = image;
                    }
                }
                catch(System.IO.FileNotFoundException ex) 
                {
                    contact.Photo = "/Images/noimagefound.jpg";
                    unitOfWork.Commit();
                    BitmapImage bitmap = new BitmapImage(new Uri("/Images/noimagefound.jpg", UriKind.Relative));
                    Photo.Source = bitmap;
                }
                
            
            }
        }

        private void SaveContactbtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string pattern1 = @"[a-zA-Z1-9\-\._]+@[a-z1-9]+(.[a-z1-9]+){1,}";
                string pattern2 = @"^\d{11}$";
                if (!Regex.IsMatch(EmailTextBox.Text, pattern1) && EmailTextBox.Text != "")
                {
                    MessageBox.Show("Некорректный email");
                }
                else if(!Regex.IsMatch(PhoneTextBox.Text, pattern2) && PhoneTextBox.Text != "")
                {
                    MessageBox.Show("Некорретно введен номер телефона.\nПридерживайтесь шаблона.");
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
                        if (DateOfBirthDP.Text != "")
                        {
                            contact.DateOfBirth = Convert.ToDateTime(DateOfBirthDP.Text);
                        }
                        contact.Country = CountryTextBox.Text;
                        contact.City = CityTextBox.Text;
                        contact.Telephone = PhoneTextBox.Text;
                        contact.Email = EmailTextBox.Text;
                        if (contact.Photo != "/Images/noimagefound.jpg" && bitmap1 != null)
                        {
                            contact.Photo = bitmap1.UriSource.LocalPath;
                            unitOfWork.Commit();
                            MessageBox.Show("Изменения прошли успешно");
                        }                        
                        else if (imgLocation != "")
                        {
                            contact.Photo = imgLocation;
                            unitOfWork.Commit();
                            MessageBox.Show("Изменения прошли успешно");
                        }
                        else if (contact.Photo == "")
                        {
                            contact.Photo = "/Images/noimagefound.jpg";
                            unitOfWork.Commit();
                        }
                        else
                        {
                            unitOfWork.Commit();
                        }
                    }
                    else if (NameTextBox.Text != "" || SurnameTextBox.Text != "" || CountryTextBox.Text != "" || CityTextBox.Text != "" || PhoneTextBox.Text != "" || EmailTextBox.Text != "")
                    {
                        var dbContext = new BaseDbContext();
                        var unitOfWork = new UnitOfWork(dbContext);

                        var user = unitOfWork.UserRepository.Entities
                                   .FirstOrDefault(n => n.Login == Login);

                        var contact = new Contact();
                        contact.Name = NameTextBox.Text;
                        contact.Surname = SurnameTextBox.Text;
                   
                        if(DateOfBirthDP.Text != "")
                        {
                            DateTime birth = Convert.ToDateTime(DateOfBirthDP.Text);
                            contact.DateOfBirth = birth;
                        }
                        
                        contact.Country = CountryTextBox.Text;
                        contact.City = CityTextBox.Text;
                        contact.Telephone = PhoneTextBox.Text;
                        contact.Email = EmailTextBox.Text;
                        contact.Id_User = user.Id;
                        if (imgLocation == "")
                        {
                            contact.Photo = "/Images/noimagefound.jpg";
                        }
                        else
                        {
                            contact.Photo = imgLocation;
                        }                      

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

        private void CancelContactbtn_Click(object sender, RoutedEventArgs e)
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
                imgLocation = op.FileName.ToString();
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

    }
}








//private BitmapImage fromByteToBitmap(Byte[] bytes)
//{
//    using (var ms = new System.IO.MemoryStream(bytes))
//    {
//        using (var imgg = System.Drawing.Image.FromStream(ms))
//        {
//            var s = new MemoryStream();
//            imgg.Save(s, System.Drawing.Imaging.ImageFormat.Gif);
//            var bi = new BitmapImage();
//            bi.BeginInit();
//            bi.StreamSource = s;
//            bi.EndInit();
//            return bi;
//        }
//    }
//}
