using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using TrafficViolationFeedbackSystem.Controllers;
using TrafficViolationFeedbackSystem.DAO;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Models;
using TrafficViolationFeedbackSystem.Services;
namespace TrafficViolationFeedbackSystem.Views.Citizen
{
    public partial class ReportViolationView : UserControl
    {
        private readonly TrafficViolationFeedbackSystemContext _context;
        private string _selectedFilePath;
        private readonly ReportController _reportController;
        private readonly ViolationTypeController _violationTypesController;
        private readonly UserController _userController;
        private readonly EmailService _emailService = new EmailService();


        public ReportViolationView(TrafficViolationFeedbackSystemContext context)
        {
            InitializeComponent();
            _context = context;
            _violationTypesController = new ViolationTypeController(_context);
            _reportController = new ReportController(_context);
            _userController = new UserController(_context);
            LoadViolationTypes();
        }

        // Load danh sách loại vi phạm từ DB
        private void LoadViolationTypes()
        {
            var violationTypes = _violationTypesController.GetAll();
            if (violationTypes != null && violationTypes.Any())
            {
                violationTypes.Insert(0, new ViolationType
                {
                    ViolationTypeId = -1,
                    Name = "-- Chọn loại vi phạm --"
                });
                cbViolationType.ItemsSource = violationTypes;
                cbViolationType.DisplayMemberPath = "Name";
                cbViolationType.SelectedValuePath = "ViolationTypeID";
                cbViolationType.SelectedIndex = 0; // hiển thị dòng mặc định
            }
            else
            {
                MessageBox.Show("Không có loại vi phạm nào được cấu hình trong hệ thống.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btnChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Ảnh và video (*.jpg;*.jpeg;*.png;*.mp4)|*.jpg;*.jpeg;*.png;*.mp4";
            openFileDialog.Title = "Chọn file minh chứng";

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFilePath = openFileDialog.FileName;
                txtFileName.Text = System.IO.Path.GetFileName(_selectedFilePath);
            }
        }

        // Xử lý khi nhấn nút Gửi phản ánh
        private async void btnSubmitReport_Click(object sender, RoutedEventArgs e)
        {
            btnSubmitReport.IsEnabled = false;
            try
            {
                string plateNumber = txtPlateNumber.Text?.Trim();
                string location = txtLocation.Text?.Trim();
                string description = txtDescription.Text?.Trim();
                var selectedType = cbViolationType.SelectedItem as ViolationType;
                // Kiểm tra bắt buộc các trường: biển số, loại vi phạm, địa điểm
                if (string.IsNullOrWhiteSpace(plateNumber))
                {
                    MessageBox.Show("Vui lòng nhập biển số xe.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    Models.User owner = await _userController.GetVehicleOwnerByPlateNumber(plateNumber);
                    if (owner == null)
                    {
                        MessageBox.Show("Biển số xe không tồn tại trong hệ thống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if (selectedType == null || selectedType.ViolationTypeId == -1)
                {
                    MessageBox.Show("Vui lòng chọn loại vi phạm.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(location))
                {
                    MessageBox.Show("Vui lòng nhập địa điểm xảy ra vi phạm.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra định dạng biển số xe
                if (!Regex.IsMatch(plateNumber, @"^[0-9]{2}[A-Z][0-9]?-([0-9]{4,5})$", RegexOptions.IgnoreCase))
                {
                    MessageBox.Show("Biển số xe không đúng định dạng!\nVí dụ hợp lệ: 30A-12345 hoặc 29B2-20102.", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kiểm tra người gửi
                if (!int.TryParse(AuthenticationContext.UserId, out int reporterId))
                {
                    MessageBox.Show("Không xác định được người gửi phản ánh.", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Kiểm tra đính kèm file
                if (string.IsNullOrEmpty(_selectedFilePath))
                {
                    MessageBox.Show("Vui lòng đính kèm file minh chứng (ảnh hoặc video).", "Thiếu file đính kèm", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                bool isMyVehicle = await IsMyVehicleAsync(plateNumber, reporterId);
                if (isMyVehicle)
                {
                    MessageBox.Show("Bạn không thể tự phản ánh xe của chính mình.", "Không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                bool reportExists = await _context.Reports.AnyAsync(r =>
                    r.ReporterId == reporterId &&
                    r.PlateNumber == plateNumber &&
                    (r.Status == "Pending" || r.Status == "Approved" || r.Status == "Rejected"));

                if (reportExists)
                {
                    MessageBox.Show("Bạn đã gửi phản ánh cho biển số này và đang chờ xử lý.\nVui lòng chờ kết quả hoặc hủy phản ánh cũ trước khi gửi lại.", "Trùng phản ánh", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ClearForm();
                    return;
                }

                var newReport = new Report
                {
                    ReporterId = reporterId,
                    PlateNumber = plateNumber,
                    ViolationTypeId = selectedType.ViolationTypeId,
                    Description = description,
                    Location = location,
                    ReportDate = DateTime.Now,
                    Status = "Pending"
                };
                await _reportController.SubmitReport(newReport, _selectedFilePath);

                // Gửi email xác nhận cho người gửi báo cáo
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == reporterId);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    string subject = "Xác nhận gửi phản ánh vi phạm giao thông";
                    string body = $"Chào {user.FullName},\n\nPhản ánh của bạn về phương tiện {plateNumber} đã được gửi thành công và đang chờ xử lý.\n\nCảm ơn bạn đã đóng góp cho cộng đồng!";
                    await _emailService.SendEmailAsync(user.Email, subject, body);
                }

                MessageBox.Show("Phản ánh của bạn đã được gửi thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gửi phản ánh: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnSubmitReport.IsEnabled = true;
            }
        }
        public async Task<bool> IsMyVehicleAsync(string plateNumber, int currentUserId)
        {
            if (string.IsNullOrWhiteSpace(plateNumber)) return false;

            // Tìm xe có biển số đó và so sánh chủ sở hữu
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.PlateNumber == plateNumber);

            return vehicle != null && vehicle.OwnerId == currentUserId;
        }


        // Hàm reset form
        private void ClearForm()
        {
            txtPlateNumber.Text = string.Empty;
            txtLocation.Text = string.Empty;
            txtDescription.Text = string.Empty;
            cbViolationType.SelectedIndex = 0;
            _selectedFilePath = string.Empty;
            txtFileName.Text = "Chưa chọn file";
        }
    }
}
