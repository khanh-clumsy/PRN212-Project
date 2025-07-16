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
        private readonly int? _appealId;
        public SendAppealWindow(int violationId, int userId)
        {
            InitializeComponent();
            _violationId = violationId;
            _userId = userId;
            var appealDAO = new AppealDAO();
            string summary = appealDAO.GetViolationSummary(_violationId);
            txtSummary.Text = summary;
        }
        public SendAppealWindow(int violationId, int userId, int appealId, string existingContent)
        {
            InitializeComponent();
            _violationId = violationId;
            _userId = userId;
            _appealId = appealId;
            txtContent.Text = existingContent;
        }
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            string content = txtContent.Text.Trim();

            if (string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("Vui lòng nhập nội dung kháng cáo!");
                return;
            }

            var dao = new AppealDAO();

            // ❗Chỉ check tồn tại nếu đang thêm mới
            if (_appealId == null && dao.AppealExists(_violationId))
            {
                MessageBox.Show("Bạn đã gửi kháng cáo cho vi phạm này rồi!");
                return;
            }

            if (_appealId != null)
            {
                // Cập nhật nội dung kháng cáo
                dao.UpdateAppeal(_appealId.Value, content); // bạn chỉ cần thêm 1 hàm Update
            }

            else
            {
                // Thêm mới
                var appeal = new Appeal
                {
                    ViolationId = _violationId,
                    ViolatorId = _userId,
                    Content = content,
                    SubmitDate = DateTime.Now,
                    Result = "Pending"
                };

                dao.AddAppeal(appeal);
            }

            MessageBox.Show("Kháng cáo đã được lưu!");
            this.Close();
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
