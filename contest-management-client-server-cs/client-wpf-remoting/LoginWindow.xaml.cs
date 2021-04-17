using System.Windows;
using client_wpf_remoting.controller;
using services;

namespace client_wpf_remoting
{
    public partial class LoginWindow : Window
    {
        private ContestMgmtClientController _controller;
        
        public LoginWindow(ContestMgmtClientController controller)
        {
            InitializeComponent();
            _controller = controller;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordTextBox.Password;
            try
            {
                _controller.Login(username, password);

                var window = new MainWindow(_controller) {Title = $"Organiser {username}"};
                window.Show();
                Hide();
            }
            catch (ContestMgmtException exception)
            {
                MessageBox.Show(this, exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}