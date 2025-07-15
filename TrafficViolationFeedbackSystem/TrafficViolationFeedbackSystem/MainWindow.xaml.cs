using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrafficViolationFeedbackSystem.Views.Authentication;

namespace TrafficViolationFeedbackSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Khởi động với màn hình đăng nhập
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            
            // Ẩn MainWindow
            this.Hide();
        }
    }
}