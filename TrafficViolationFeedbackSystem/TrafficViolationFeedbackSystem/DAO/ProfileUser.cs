using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TrafficViolationFeedbackSystem.DAO;
using TrafficViolationFeedbackSystem.Data;

namespace TrafficViolationFeedbackSystem.Views.Citizen
{
    public partial class ProfileUser : UserControl
    {

        private readonly UserDAO _userDAO;
        private readonly int _currentUserId;

        public ProfileUser(int userId)
        {
            InitializeComponent();
            _currentUserId = userId;
            _userDAO = new UserDAO(new TrafficViolationFeedbackSystemContext());
            LoadUserProfile();
        }

        private void LoadUserProfile()
        {
            try
            {
                var user = _userDAO.GetUserById(_currentUserId);
                if (user != null)
                {
                    txtFullName.Text = user.FullName;
                    txtEmail.Text = user.Email;
                    txtPhone.Text = user.Phone ?? "Chưa cập nhật";
                    txtAddress.Text = user.Address ?? "Chưa cập nhật";
                    LoadVehicles();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin người dùng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin hồ sơ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadVehicles()
        {
            try
            {
                var vehicles = _userDAO.GetVehiclesByOwnerId(_currentUserId);
                if (vehicles == null || vehicles.Count == 0)
                {
                    dgVehicles.ItemsSource = null;
                    MessageBox.Show("Không có phương tiện nào được đăng ký cho người dùng này.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    dgVehicles.ItemsSource = vehicles;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách phương tiện: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra FullName không rỗng
                if (string.IsNullOrWhiteSpace(txtFullName.Text))
                {
                    MessageBox.Show("Họ và tên không được để trống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // Kiểm tra định dạng Họ và tên: chỉ chứa chữ cái Unicode và dấu cách
                if (!Regex.IsMatch(txtFullName.Text, @"^[\p{L}\s]+$"))
                {
                    MessageBox.Show("Họ và tên chỉ được chứa chữ cái và dấu cách, không chứa ký tự đặc biệt hoặc số.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Kiểm tra định dạng Email: phải là @gmail.com
                if (!Regex.IsMatch(txtEmail.Text, @"^[a-zA-Z0-9._%+-]+@gmail\.com$"))
                {
                    MessageBox.Show("Email phải có định dạng @gmail.com.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Kiểm tra định dạng Số điện thoại: 10 số, bắt đầu bằng 0 (nếu không rỗng)
                if (!string.IsNullOrWhiteSpace(txtPhone.Text) && txtPhone.Text != "Chưa cập nhật")
                {
                    if (!Regex.IsMatch(txtPhone.Text, @"^0\d{9}$"))
                    {
                        MessageBox.Show("Số điện thoại phải có đúng 10 số và bắt đầu bằng 0.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                
                var user = _userDAO.GetUserById(_currentUserId);
                if (user != null)
                {
                    user.FullName = txtFullName.Text;
                    user.Phone = string.IsNullOrWhiteSpace(txtPhone.Text) || txtPhone.Text == "Chưa cập nhật" ? null : txtPhone.Text;
                    user.Address = string.IsNullOrWhiteSpace(txtAddress.Text) || txtAddress.Text == "Chưa cập nhật" ? null : txtAddress.Text;
                    var (success, message) = _userDAO.UpdateUser(user);
                    if (success)
                    {
                        MessageBox.Show(message, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin người dùng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật thông tin hồ sơ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
