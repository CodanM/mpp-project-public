using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using client_wpf.controller;
using model;
using services;

namespace client_wpf
{
    public partial class RegistrationWindow : Window
    {
        private readonly ContestMgmtClientController _controller;

        private MainWindow _mainWindow;
        
        private readonly IList<string> _competitionTypes;
        
        private IList<string> _ageCategories;
        
        public RegistrationWindow(ContestMgmtClientController controller, MainWindow mainWindow)
        {
            InitializeComponent();
            
            _controller = controller;
            _mainWindow = mainWindow;
            
            _competitionTypes = controller.GetCompetitionTypes();
            Dispatcher.BeginInvoke(() => CompetitionTypeComboBox.ItemsSource = _competitionTypes);
        }

        private void DecrementAgeButton_Click(object sender, RoutedEventArgs e)
        {
            var age = int.Parse(AgeTextBlock.Text);
            if (age <= 6) return;

            age--;
            Dispatcher.BeginInvoke(() =>
            {
                if (age == 6)
                    DecrementAgeButton.IsEnabled = false;
                AgeTextBlock.Text = age.ToString();
                IncrementAgeButton.IsEnabled = true;
                UpdateAgeCategories();
            });
        }

        private void IncrementAgeButton_Click(object sender, RoutedEventArgs e)
        {
            var age = int.Parse(AgeTextBlock.Text);
            if (age >= 15) return;

            age++;
            Dispatcher.BeginInvoke(() =>
            {
                if (age == 15)
                    IncrementAgeButton.IsEnabled = false;
                AgeTextBlock.Text = age.ToString();
                DecrementAgeButton.IsEnabled = true;
                UpdateAgeCategories();
            });
        }

        private void ExistsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                AgeTextBlock.Visibility = Visibility.Hidden;
                IncrementAgeButton.IsEnabled = false;
                DecrementAgeButton.IsEnabled = false;
                IdComboBox.IsEnabled = true;
            });
        }

        private void ExistsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                AgeTextBlock.Visibility = Visibility.Visible;
                IncrementAgeButton.IsEnabled = AgeTextBlock.Text != "15";
                DecrementAgeButton.IsEnabled = AgeTextBlock.Text != "6";
                IdComboBox.IsEnabled = false;
                IdComboBox.SelectedItem = null;
            });
        }

        private void CompetitionTypeComboBox_DropDownClosed(object sender, EventArgs e)
        {
            UpdateAgeCategories();
        }

        private void UpdateAgeCategories()
        {
            if (string.IsNullOrEmpty(CompetitionTypeComboBox.SelectedItem?.ToString()))
            {
                _ageCategories = new List<string>();
                AgeCategoryComboBox.ItemsSource = _ageCategories;
                return;
            }

            var competitionType = CompetitionTypeComboBox.SelectedItem.ToString();

            long? id;
            if (string.IsNullOrEmpty(IdComboBox.Text)) id = null;
            else id = long.Parse(IdComboBox.SelectedItem.ToString() ?? "");

            int? age = id == null ? int.Parse(AgeTextBlock.Text) : null;

            _ageCategories = _controller.GetAgeCategoriesForParticipant(competitionType, id, age);
            Dispatcher.BeginInvoke(() => AgeCategoryComboBox.ItemsSource = _ageCategories);
        }

        private void IdComboBox_DropDownOpened(object sender, EventArgs e)
        {
            var firstName = FirstNameTextBox.Text;
            var lastName = LastNameTextBox.Text;
            var ids = _controller.GetParticipantIdsByName(firstName, lastName);
            Dispatcher.BeginInvoke(() => IdComboBox.ItemsSource = ids);
        }
        
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FirstNameTextBox.Text) || string.IsNullOrEmpty(LastNameTextBox.Text) ||
                string.IsNullOrEmpty(CompetitionTypeComboBox.SelectedItem?.ToString()) ||
                string.IsNullOrEmpty(AgeCategoryComboBox.SelectedItem?.ToString()))
                return;
            var participantId = long.Parse(IdComboBox.SelectedItem?.ToString() ?? "0");
            var firstName = FirstNameTextBox.Text;
            var lastName = LastNameTextBox.Text;
            var age = participantId == 0 ? int.Parse(AgeTextBlock.Text) : 0;
            var competitionType = CompetitionTypeComboBox.SelectedItem.ToString();
            var ageCategory = AgeCategoryComboBox.SelectedItem.ToString();
            try
            {
                _controller.AddRegistration(participantId, firstName, lastName, age, competitionType, ageCategory);
            }
            catch (ContestMgmtException exception)
            {
                MessageBox.Show(this, exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}