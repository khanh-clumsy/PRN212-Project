using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Services;
using TrafficViolationFeedbackSystem.Views.Authentication;

namespace TrafficViolationFeedbackSystem.Views.Citizen
{
    /// <summary>
    /// Interaction logic for CitizenDashboardWindow.xaml
    /// </summary>
    public partial class CitizenDashboardWindow : Window
    {
        private readonly TrafficViolationFeedbackSystemContext _context = new TrafficViolationFeedbackSystemContext(); 
        public CitizenDashboardWindow()
        {
            InitializeComponent();
            var btn = btnHome;
            btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007BFF"));
            btn.Foreground = Brushes.White;
            SetContent(new HomeView(SetContent, _context));
            MessageBox.Show($"Chào mừng bạn đến với Dashboard của Citizen! ID user: {AuthenticationContext.UserId}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void SetContent(UserControl view)
        {
            MainContent.Content = view;
        }

        private void ResetSidebarButtonStyles()
        {
            var buttons = new[] { btnHome, btnProfile, btnStatistics, btnLogout };
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
            SetContent(new HomeView(SetContent, _context));
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Xử lý mở Hồ sơ cá nhân
            ResetSidebarButtonStyles();
            var btn = sender as Button;
            btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007BFF"));
            btn.Foreground = Brushes.White;
            SetContent(new ProfileView());
        }

        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Xử lý xem Thống kê
            ResetSidebarButtonStyles();
            var btn = sender as Button;
            btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007BFF"));
            btn.Foreground = Brushes.White;
            SetContent(new StatisticsView());
        }


        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                // Quay về màn hình đăng nhập
                var loginWindow = new LoginWindow();
                loginWindow.Show();

                // Đóng dashboard
                this.Close();
            }
        }
    }
}
