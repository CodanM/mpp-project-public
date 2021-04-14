using System.Data;
using System.Windows;
using contest_management_gui_cs.service;

namespace contest_management_gui_cs
{
    public partial class MainWindow : Window
    {
        private ContestManagementService _service;
        public ContestManagementService Service
        {
            private get => _service;
            init
            {
                _service = value;
                var comps = Service.GetCompetitionsByString("");
                CompetitionTypesListBox.ItemsSource = comps;
            } 
        }
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var comps = Service.GetCompetitionsByString(CompetitionTypeTextBox.Text);
            CompetitionTypesListBox.ItemsSource = comps;
        }

        private void CompetitionTypesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CompetitionTypesListBox.SelectedItems.Count == 0) return;
            
            var s = CompetitionTypesListBox.SelectedItems[0]?.ToString();
            var ageCategories = Service.GetAgeCategoriesByCompetitionType(s);
            AgeCategoriesListBox.ItemsSource = ageCategories;
            CountRegisteredLabel.Content = "";
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (CompetitionTypesListBox.SelectedItems.Count == 0 ||
                AgeCategoriesListBox.SelectedItems.Count == 0) return;
            
            var competitionType = CompetitionTypesListBox.SelectedItems[0]?.ToString();
            var ageCategory = AgeCategoriesListBox.SelectedItems[0]?.ToString();
            var participants = Service.GetParticipantsByCompetition(competitionType, ageCategory);
            ParticipantsDataGrid.ItemsSource = participants;
            foreach (var col in ParticipantsDataGrid.Columns)
                switch (col.Header)
                {
                    case "Id":
                        col.DisplayIndex = 0;
                        col.Header = "ID";
                        break;
                    case "FirstName":
                        col.DisplayIndex = 1;
                        col.Header = "First Name";
                        break;
                    case "LastName":
                        col.DisplayIndex = 2;
                        col.Header = "Last Name";
                        break;
                    case "Age":
                        col.DisplayIndex = 3;
                        break;
                }
        }

        private void AgeCategoriesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CompetitionTypesListBox.SelectedItems.Count == 0 ||
                AgeCategoriesListBox.SelectedItems.Count == 0) return;
            
            var competitionType = CompetitionTypesListBox.SelectedItems[0]?.ToString();
            var ageCategory = AgeCategoriesListBox.SelectedItems[0]?.ToString();
            int count = Service.CountParticipantsForCompetition(competitionType, ageCategory);
            CountRegisteredLabel.Content = $"{count} registered";
        }

        private void RegisterParticipantButton_Click(object sender, RoutedEventArgs e)
        {
            var rnpw = new RegisterNewParticipantWindow {Service = Service};
            rnpw.Show();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var lw = new LoginWindow();
            lw.Show();
            Close();
        }
    }
}