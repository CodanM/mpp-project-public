using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Windows;
using client_wpf_remoting.controller;
using services;

namespace client_wpf_remoting
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConsoleAllocator.ShowConsoleWindow();

            var serverProvider = new BinaryServerFormatterSinkProvider {TypeFilterLevel = TypeFilterLevel.Full};
            var clientProvider = new BinaryClientFormatterSinkProvider();
            IDictionary props = new Hashtable();

            props["port"] = 0;
            var channel = new TcpChannel(props, clientProvider, serverProvider);
            ChannelServices.RegisterChannel(channel, false);
            var services =
                (IContestMgmtServices) Activator.GetObject(typeof(IContestMgmtServices),
                    "tcp://localhost:2602/ContestMgmt");
            Console.WriteLine("Reference obtained");

            var controller = new ContestMgmtClientController(services);
            var window = new LoginWindow(controller);
            window.Show();
            //Console.ReadKey();
        }
    }
    
    internal static class ConsoleAllocator
    {
        [DllImport(@"kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport(@"kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport(@"user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SwHide = 0;
        const int SwShow = 5;


        public static void ShowConsoleWindow()
        {
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
            {
                AllocConsole();
            }
            else
            {
                ShowWindow(handle, SwShow);
            }
        }

        public static void HideConsoleWindow()
        {
            var handle = GetConsoleWindow();

            ShowWindow(handle, SwHide);
        }
    }
}