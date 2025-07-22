using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using TrafficViolationFeedbackSystem.Models;
using System.Text.RegularExpressions;
using TrafficViolationFeedbackSystem.Views.Admin;
using TrafficViolationFeedbackSystem.Data;

namespace TrafficViolationFeedbackSystem
{
    public class UserCreateViewModel : BaseViewModel
    {
        private User _newUser = new User();

        public User NewUser
        {
            get => _newUser;
            set
            {
                _newUser = value;
                OnPropertyChanged(nameof(NewUser));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand SubmitCommand { get; }

        public UserCreateViewModel()
        {
            SubmitCommand = new RelayCommand(Submit, CanSubmit);
        }

        private bool CanSubmit(object parameter)
        {
            // Kiểm tra các trường bắt buộc
            bool isValid = !string.IsNullOrWhiteSpace(NewUser.FullName) &&
                           !string.IsNullOrWhiteSpace(NewUser.Email) &&
                           !string.IsNullOrWhiteSpace(NewUser.Password) &&
                           !string.IsNullOrWhiteSpace(NewUser.Role);

            // Kiểm tra Role hợp lệ
            bool isValidRole = NewUser.Role == "Citizen" || NewUser.Role == "TrafficPolice" || NewUser.Role == "Admin";

            return isValid && isValidRole;
        }

        private void Submit(object parameter)
        {
            try
            {
                using (var context = new TrafficViolationFeedbackSystemContext())
                {
                    // Kiểm tra duy nhất
                    if (!Regex.IsMatch(NewUser.FullName, @"^[\p{L}\s]+$"))
                    { 
                        MessageBox.Show("Họ tên chỉ được chứa chữ cái và dấu cách.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                     }

                    if (!Regex.IsMatch(NewUser.Email, @"^[a-zA-Z0-9._%+-]+@gmail\.com$"))
                    {
                        MessageBox.Show("Email phải có định dạng @gmail.com.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (context.Users.Any(u => u.Email == NewUser.Email))
                    {
                        MessageBox.Show("Email đã tồn tại. Vui lòng chọn email khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if(!Regex.IsMatch(NewUser.Phone, @"^0[0-9]{9}$"))
                    {
                        MessageBox.Show("Số điện thoại chỉ được chứa 10 số & bắt đầu từ 0.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (context.Users.Any(u => u.Phone == NewUser.Phone))
                    {
                        MessageBox.Show("Phone đã tồn tại. Vui lòng chọn Phone khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Thêm người dùng mới
                    var user = new User
                    {
                        FullName = NewUser.FullName,
                        Email = NewUser.Email,
                        Password = NewUser.Password,
                        Role = NewUser.Role,
                        Phone = NewUser.Phone,
                        Address = NewUser.Address
                    };
                    context.Users.Add(user);
                    context.SaveChanges();

                    MessageBox.Show("Người dùng đã được tạo thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Quay lại UserManagementView
                    var mainWindow = Application.Current.MainWindow?.DataContext as AdminDashboardModel;
                    if (mainWindow != null)
                    {
                        mainWindow.CurrentView = new UserManagementView { DataContext = new UserManagementViewModel() };
                        mainWindow.ImageVisibility = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo người dùng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
