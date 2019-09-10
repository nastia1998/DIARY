using DIARY_V4.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginEnterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dbContext = new BaseDbContext();
                var unitOfWork = new UnitOfWork(dbContext);

                var user = unitOfWork.UserRepository.Entities
                    .FirstOrDefault (n => (n.Login == LoginTextBox.Text) && (n.Password == LoginFloatingPasswordBox.Password.ToString()));

                if (user != null)
                {                    
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Login = LoginTextBox.Text;
                    mainWindow.Name = user.Name;
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неправильно набран логин или пароль");
                }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoginRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void ForgetPassword_Button_Click(object sender, RoutedEventArgs e)
        {
            ForgetPasswordWindow forgetPasswordWindow = new ForgetPasswordWindow();
            forgetPasswordWindow.Show();
            this.Close();
        }
    }
}
