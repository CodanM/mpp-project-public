using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using client_wpf.controller;
using model;
using networking.dto;

namespace client_wpf
{
    public partial class MainWindow : Window
    {
        private readonly ContestMgmtClientController _controller;

        private IList<CompetitionCountDTO> _competitions;

        private IList<Participant> _participants;

        public delegate void UpdateDataGridCallback(DataGrid dataGrid, string competitionType, string ageCategory);
        
        public MainWindow(ContestMgmtClientController controller)
        {
            InitializeComponent();
            _controller = controller;
            _competitions = controller.GetCompetitionsAndCountsByString("", "");
            CompetitionsDataGrid.DataContext = _competitions;
        }

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
            _competitions = _controller.GetCompetitionsAndCountsByString(competitionType, ageCategory);
            CompetitionsDataGrid.ItemsSource = _competitions;
        }

        private void CompetitionsDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (CompetitionsDataGrid.SelectedCells.Count == 0)
            {
                _participants = new List<Participant>();
                ParticipantsDataGrid.ItemsSource = _participants;
                return;
            }

            var ccDto = (CompetitionCountDTO)CompetitionsDataGrid.SelectedCells[0].Item;
            _participants = _controller.GetParticipantsByCompetition((((Competition, int)) ccDto).Item1);
            ParticipantsDataGrid.ItemsSource = _participants;
        }
    }
}