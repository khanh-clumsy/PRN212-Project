using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Views.Authentication;
using TrafficViolationFeedbackSystem.Views.Citizen;

namespace TrafficViolationFeedbackSystem.Views.TrafficPolice
{
    public partial class TrafficPoliceDashboardWindow : Window
    {
        private readonly TrafficViolationFeedbackSystemContext _context = new TrafficViolationFeedbackSystemContext();
        private readonly int _currentUserId;
        public TrafficPoliceDashboardWindow(int userId)
        {
            InitializeComponent();
            _currentUserId = userId;
            var btn = btnHome;
            btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007BFF"));
            btn.Foreground = Brushes.White;
            SetContent(new TrafficPoliceHomeView(SetContent, _context));
        }

        public void SetContent(UserControl view)
        {
            MainContent.Content = view;
        }

        private void ResetSidebarButtonStyles()
        {
            var buttons = new[] { btnHome, btnProfile, btnLogout };
            foreach (var btn in buttons)
            {
                btn.Background = Brushes.Transparent;
                btn.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007BFF"));
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            ResetSidebarButtonStyles();
            var btn = sender as Button;
            btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007BFF"));
            btn.Foreground = Brushes.White;
            SetContent(new TrafficPoliceHomeView(SetContent, _context));
        }
        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Xử lý mở Hồ sơ cá nhân
            ResetSidebarButtonStyles();
            var btn = sender as Button;
            btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007BFF"));
            btn.Foreground = Brushes.White;
            SetContent(new ProfileUser(_currentUserId));
        }
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
} 