using DIARY_V4.Model;
using System;
using System.Linq;
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
                if (NameTextBox.Text != "" && RegisterLoginTextBox.Text != "" && RegisterFloatingPasswordBox1.Password != "" && RegisterFloatingPasswordBox2.Password != "" && RegisterSecretWordTextBox.Text != "")
                {
                    if (RegisterFloatingPasswordBox1.Password == RegisterFloatingPasswordBox2.Password)
                    {
                        var userRepeate = unitOfWork.UserRepository.Entities
                                    .FirstOrDefault(b => b.Login == RegisterLoginTextBox.Text);
                        var secrwRepeate = unitOfWork.UserRepository.Entities
                                    .FirstOrDefault(n => n.SecretWord == RegisterSecretWordTextBox.Text);
                        if (userRepeate == null)
                        {
                            if (secrwRepeate == null)
                            {
                                var user = new User() { Name = NameTextBox.Text, Login = RegisterLoginTextBox.Text, Password = RegisterFloatingPasswordBox1.Password, SecretWord = RegisterSecretWordTextBox.Text };

                                unitOfWork.UserRepository.Add(user);
                                unitOfWork.Commit();

                                MessageBox.Show("Регистрация прошла успешно");

                                LoginWindow loginWindow = new LoginWindow();
                                loginWindow.Show();
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Придумайте другое секретное слово!", "Новое секретное слово", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Пользователь с таким логином уже существует!\nИспользуйте другой логин.", "Повторение логина", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Пароли в полях должны совпадать");
                    }
                }
                else
                {
                    MessageBox.Show("Все поля должны быть заполнены", "Пустые поля", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
