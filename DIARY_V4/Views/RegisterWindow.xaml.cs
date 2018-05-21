using DIARY_V4.Model;
using System;
using System.Windows;

namespace DIARY_V4
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dbContext = new BaseDbContext();
                var unitOfWork = new UnitOfWork(dbContext);

                if(RegisterFloatingPasswordBox1.Password.ToString() == RegisterFloatingPasswordBox2.Password.ToString())
                {
                    var user = new User() { Name = NameTextBox.Text, Login = RegisterLoginTextBox.Text, Password = RegisterFloatingPasswordBox1.Password.ToString(), SecretWord = RegisterSecretWordTextBox.Text };

                    unitOfWork.UserRepository.Add(user);
                    unitOfWork.Commit();

                    //var users = unitOfWork.UserRepository.Entities;
                    //foreach(var u in users)
                    //{
                    //    MessageBox.Show(u.Login + u.Name  + "\n");
                    //}
                    
                    MessageBox.Show("Пользователь добавлен");

                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Пароли в полях должны совпадать");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
