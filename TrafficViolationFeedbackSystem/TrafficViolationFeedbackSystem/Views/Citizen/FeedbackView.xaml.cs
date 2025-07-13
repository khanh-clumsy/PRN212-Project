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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrafficViolationFeedbackSystem.DAO;

namespace TrafficViolationFeedbackSystem.Views.Citizen
{
    /// <summary>
    /// Interaction logic for FeedbackView.xaml
    /// </summary>
    public partial class FeedbackView : UserControl
    {
        private int userId = 1;

        public FeedbackView()
        {
            InitializeComponent();
            LoadAppeals();
        }

        private void LoadAppeals()
        {
            var dao = new AppealDAO();
            var data = dao.GetAppealsByUser(userId);
            dgAppeals.ItemsSource = data;
        }

        private void EditAppeal_Click(object sender, RoutedEventArgs e)
        {
            var dao = new AppealDAO();
            var button = sender as Button;
            if (button?.Tag == null) return;

            int appealId = Convert.ToInt32(button.Tag);

            // Kiểm tra lại nếu appeal đã xử lý (an toàn phía backend)
            var appeal = dao.GetAppealById(appealId);
            if (appeal == null)
            {
                MessageBox.Show("Không tìm thấy kháng cáo.");
                return;
            }

            if (!string.IsNullOrEmpty(appeal.Result))
            {
                MessageBox.Show("Kháng cáo đã được xử lý, không thể sửa.");
                return;
            }

            // Hiển thị form chỉnh sửa
            var editWindow = new SendAppealWindow(appeal); // constructor có thể truyền dữ liệu
            editWindow.ShowDialog();

            // Sau khi sửa xong → load lại
            LoadAppeals();
        }

        private void DeleteAppeal_Click(object sender, RoutedEventArgs e)
        {
            var dao = new AppealDAO();
            var button = sender as Button;
            if (button?.Tag == null) return;

            int appealId = Convert.ToInt32(button.Tag);

            var appeal = dao.GetAppealById(appealId);
            if (appeal == null)
            {
                MessageBox.Show("Không tìm thấy kháng cáo.");
                return;
            }

            if (!string.IsNullOrEmpty(appeal.Result))
            {
                MessageBox.Show("Kháng cáo đã được xử lý, không thể xóa.");
                return;
            }

            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa kháng cáo này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                bool success = dao.DeleteAppeal(appealId);
                if (success)
                {
                    MessageBox.Show("Đã xóa thành công.");
                    LoadAppeals(); // Load lại danh sách
                }
                else
                {
                    MessageBox.Show("Xóa thất bại. Vui lòng thử lại.");
                }
            }
        }


    }
}
