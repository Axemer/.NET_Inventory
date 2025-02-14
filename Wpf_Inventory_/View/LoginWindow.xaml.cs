using System.Windows;
using System.Windows.Controls;

namespace Wpf_Inventory_.View
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private static readonly string AD_Domain = "yourdomain.com";
        private readonly ActiveDirectoryAuth _auth = new ActiveDirectoryAuth(AD_Domain);

        public bool IsAuthenticated { get; private set; } = false;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;

            if (_auth.AuthenticateUser(username, password))
            {
                IsAuthenticated = true;
                MessageBox.Show("Успешный вход!", "Доступ разрешен", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Ошибка входа! Проверьте логин и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
