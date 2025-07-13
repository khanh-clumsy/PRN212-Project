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
using System.Windows.Controls.Primitives;

namespace TrafficViolationFeedbackSystem
{
    /// <summary>
    /// Interaction logic for UserCreateView.xaml
    /// </summary>
    public partial class UserCreateView : UserControl
    {
        public UserCreateView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is UserCreateViewModel viewModel)
            {
                viewModel.NewUser.Password = (sender as PasswordBox)?.Password ?? string.Empty;
                //viewModel.OnPropertyChanged(nameof(viewModel.NewUser)); // Thông báo thay đổi cho Password               
                CommandManager.InvalidateRequerySuggested(); // Đảm bảo CanExecute được gọi lại
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is UserCreateViewModel viewModel && sender is ComboBox comboBox)
            {
                var selectedItem = comboBox.SelectedItem as ComboBoxItem;
                viewModel.NewUser.Role = selectedItem?.Tag?.ToString();
                //viewModel.OnPropertyChanged(nameof(viewModel.NewUser)); // Thông báo thay đổi cho NewUser
                CommandManager.InvalidateRequerySuggested(); // Đảm bảo CanExecute được gọi lại
            }
        }
    }
}
