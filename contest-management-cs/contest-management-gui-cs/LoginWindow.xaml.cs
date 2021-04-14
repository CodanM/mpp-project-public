using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using contest_management_gui_cs.repository;
using contest_management_gui_cs.service;
using contest_management_gui_cs.utils;

namespace contest_management_gui_cs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly DbUtils _dbUtils = new("contest-mgmt-db");

        private ContestManagementService _service;

        public LoginWindow()
        {
            InitializeComponent();
            
            InitialiseService();
        }

        private void InitialiseService()
        {
            IParticipantRepository participantRepository = new ParticipantDbRepository("contest-mgmt-db");

            ICompetitionRepository competitionRepository = new CompetitionDbRepository("contest-mgmt-db");
            
            IOrganiserRepository organiserRepository = new OrganiserDbRepository("contest-mgmt-db");

            IRegistrationRepository registrationRepository = new RegistrationDbRepository("contest-mgmt-db");

            _service = new ContestManagementService
            {
                ParticipantRepository = participantRepository,
                CompetitionRepository = competitionRepository,
                OrganiserRepository = organiserRepository,
                RegistrationRepository = registrationRepository
            };
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _service.Login(UsernameTextField.Text, PasswordTextField.Password);
                var mw = new MainWindow
                {
                    Title = $"{_service.CurrentOrganiser.FirstName} {_service.CurrentOrganiser.LastName}",
                    Service = _service
                };
                mw.Show();
                Close();
            }
            catch (InvalidCredentialException)
            {
                MessageBox.Show("Invalid username or password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            PasswordTextField.Password = "";
        }
    }
}