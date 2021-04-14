using System.Windows;
using client_wpf.controller;
using networking;
using services;

namespace client_wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IContestMgmtServices server = new ContestMgmtServerRpcProxy("127.0.0.1", 2602);
            ContestMgmtClientController controller = new ContestMgmtClientController(server);

            var window = new LoginWindow(controller);
            window.Show();
        }
    }
}