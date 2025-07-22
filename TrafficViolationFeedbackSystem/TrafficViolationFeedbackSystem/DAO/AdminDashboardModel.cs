using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TrafficViolationFeedbackSystem.Models;

namespace TrafficViolationFeedbackSystem.Views.Admin
{
    public class AdminDashboardModel : BaseViewModel
    {
        private object _currentView;
        private bool _imageVisibility;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public bool ImageVisibility
        {
            get => _imageVisibility;
            set
            {
                _imageVisibility = value;
                OnPropertyChanged(nameof(ImageVisibility));
            }
        }

        public ICommand NavigateCommand { get; }
        public ICommand LogoutCommand { get; }

        public AdminDashboardModel()
        {
            NavigateCommand = new RelayCommand(Navigate);
            LogoutCommand = new RelayCommand(Logout);
            ImageVisibility = true; // Show image by default

        }

        private void Navigate(object parameter)
        {
            ImageVisibility = false; // Hide image when navigating
            var parentWindow = Application.Current.MainWindow; // Lấy cửa sổ chính
            if (parentWindow == null)
            {
                MessageBox.Show("Main window is not available. Please ensure the application is fully loaded.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ImageVisibility = true;
                return;
            }
            switch (parameter?.ToString())
            {
                case "UserManagement":
                    CurrentView = new UserManagementView { DataContext = new UserManagementViewModel() };
                    break;
                case "Report":
                    CurrentView = new ViolationStatisticsView { DataContext = new ViolationStatisticsViewModel() };
                    ImageVisibility = false;
                    break;
                case "ViolationType":
                    CurrentView = new ViolationTypesView { DataContext = new ViolationTypesViewModel(parentWindow) };
                    ImageVisibility = false;
                    break;             
                default:
                    CurrentView = null;
                    ImageVisibility = true;
                    break;
            }
        }

        private void Logout(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}