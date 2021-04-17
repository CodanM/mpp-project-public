using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using client_wpf_remoting.controller;
using model;

namespace client_wpf_remoting
{
    public partial class MainWindow : Window
    {
        private readonly ContestMgmtClientController _controller;

        private IList<CompetitionCountDTO> _competitions;

        private IList<Participant> _participants;

        public MainWindow(ContestMgmtClientController controller)
        {
            InitializeComponent();
            _controller = controller;
            _competitions = controller.GetCompetitionsAndCounts();
            Dispatcher.BeginInvoke(new Action(() => CompetitionsDataGrid.DataContext = _competitions));

            controller.UpdateEvent += OrganiserUpdate;
        }

        public delegate void UpdateRegistrationCountCallback(long id);

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine("MainWindow closing...");
            _controller.Logout();
            Application.Current.Shutdown();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var competitionType = CompetitionTypeTextBox.Text;
            var ageCategory = AgeCategoryTextBox.Text;
            _competitions = _controller.GetCompetitionsAndCounts(competitionType, ageCategory);
            Dispatcher.BeginInvoke(new Action(() => CompetitionsDataGrid.ItemsSource = _competitions));
        }

        private void CompetitionsDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (CompetitionsDataGrid.SelectedCells.Count == 0)
            {
                _participants = new List<Participant>();
                Dispatcher.BeginInvoke(new Action(() => ParticipantsDataGrid.ItemsSource = _participants));
                return;
            }

            var ccDto = (CompetitionCountDTO)CompetitionsDataGrid.SelectedCells[0].Item;
            var (comp, _) = ((Competition, int)) ccDto;
            _participants = _controller.GetParticipantsByCompetition(comp);
            Dispatcher.BeginInvoke(new Action(() => ParticipantsDataGrid.ItemsSource = _participants));
        }

        private void AddRegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new RegistrationWindow(_controller, this) {Title = "Add new registration"};
            window.Show();
        }
        
        private void OrganiserUpdate(object sender, ContestMgmtOrganiserEventArgs e)
        {
            if (e.OrganiserEventType == ContestMgmtOrganiserEvent.NewRegistration)
            {
                var id = ((Registration) e.Data).Competition.Id;
                Console.WriteLine($"New Registration: Competition Id: {id}");
                Dispatcher.BeginInvoke(new UpdateRegistrationCountCallback(UpdateRegistrationCount), id);
            }
        }
        
        private void UpdateRegistrationCount(long id)
        {
            var ccDto = _competitions.FirstOrDefault(cc => cc.CompetitionId == id);
            if (ccDto == null)
                return;
            ccDto.Count++;
            Dispatcher.BeginInvoke(new Action(() => 
            {
                CompetitionsDataGrid.ItemsSource = null;
                CompetitionsDataGrid.ItemsSource = _competitions;
            }));
        }
    }
}