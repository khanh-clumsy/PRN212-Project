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
using TrafficViolationFeedbackSystem.DAO;
using TrafficViolationFeedbackSystem.Models;

namespace TrafficViolationFeedbackSystem.Views.Citizen
{
    /// <summary>
    /// Interaction logic for SendAppealWindow.xaml
    /// </summary>
    public partial class SendAppealWindow : Window
    {
        private int _violationId;
        private int _userId;
        public SendAppealWindow(int violationId, int userId)
        {
            InitializeComponent();
            _violationId = violationId;
            _userId = userId;
            var appealDAO = new AppealDAO();
            string summary = appealDAO.GetViolationSummary(_violationId);
            txtSummary.Text = summary;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            string content = txtContent.Text.Trim();

            if (string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("Vui lòng nhập nội dung kháng cáo!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var appealDAO = new AppealDAO();
            if (appealDAO.AppealExists(_violationId))
            {
                MessageBox.Show("Bạn đã gửi kháng cáo cho vi phạm này rồi!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var appeal = new Appeal
            {
                ViolationId = _violationId,
                ViolatorId = _userId,
                Content = content,
                SubmitDate = DateTime.Now
            };

            bool success = appealDAO.AddAppeal(appeal);

            if (success)
            {
                MessageBox.Show("Đã gửi kháng cáo thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Gửi kháng cáo thất bại. Vui lòng thử lại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
