using System;
using System.Data;
using System.Windows;
using contest_management_gui_cs.service;

namespace contest_management_gui_cs
{
    public partial class RegisterNewParticipantWindow : Window
    {
        private ContestManagementService _service;
        public ContestManagementService Service
        {
            private get => _service;

            init
            {
                _service = value;
                CompetitionTypeComboBox.ItemsSource = Service.GetCompetitionsByString("");
            }
        }
        
        public RegisterNewParticipantWindow()
        {
            InitializeComponent();
        }

        private void CompetitionTypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CompetitionTypeComboBox.SelectedItem?.ToString() == "")
                return;
            var ageCategories = _service.GetAgeCategoriesByCompetitionType(CompetitionTypeComboBox.SelectedItem?.ToString());
            AgeCategoryComboBox.ItemsSource = ageCategories;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var firstName = FirstNameTextBox.Text;
            var lastName = LastNameTextBox.Text;
            var age = (int) AgeSlider.Value;
            var competitionType = CompetitionTypeComboBox.SelectedItem.ToString();
            var ageCategory = AgeCategoryComboBox.SelectedItem.ToString();
            try
            {
                _service.AddRegistration(firstName, lastName, age, competitionType, ageCategory);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            CompetitionTypeComboBox.SelectedItem = null;
        }
    }
}