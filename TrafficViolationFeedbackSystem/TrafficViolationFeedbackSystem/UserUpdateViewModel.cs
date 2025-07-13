using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using TrafficViolationFeedbackSystem.Models;

namespace TrafficViolationFeedbackSystem
{
    public class UserUpdateViewModel : BaseViewModel
    {
        private User _updatedUser;

        public User UpdatedUser
        {
            get => _updatedUser;
            set
            {
                _updatedUser = value;
                OnPropertyChanged(nameof(UpdatedUser));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand UpdateCommand { get; }

        public UserUpdateViewModel(User user)
        {
            UpdatedUser = user ?? new User();
            UpdateCommand = new RelayCommand(Update, CanUpdate);
        }      

        private bool CanUpdate(object parameter)
        {
            return !string.IsNullOrWhiteSpace(UpdatedUser.FullName) &&
                   !string.IsNullOrWhiteSpace(UpdatedUser.Email) &&
                   !string.IsNullOrWhiteSpace(UpdatedUser.Password) &&
                   !string.IsNullOrWhiteSpace(UpdatedUser.Role) &&
                   (UpdatedUser.Role == "Citizen" || UpdatedUser.Role == "TrafficPolice" || UpdatedUser.Role == "Admin");
        }

        private void Update(object parameter)
        {
            try
            {
                using (var context = new TrafficViolationFeedbackSystemContext())
                {
                    var existingUser = context.Users.FirstOrDefault(u => u.UserId == UpdatedUser.UserId);
                    if (existingUser == null)
                    {
                        MessageBox.Show("Người dùng không tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Kiểm tra email duy nhất (trừ chính User hiện tại)
                    if (context.Users.Any(u => u.Email == UpdatedUser.Email && u.UserId != UpdatedUser.UserId))
                    {
                        MessageBox.Show("Email đã tồn tại. Vui lòng chọn email khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Cập nhật thông tin
                    existingUser.FullName = UpdatedUser.FullName;
                    existingUser.Email = UpdatedUser.Email;
                    existingUser.Password = UpdatedUser.Password;
                    existingUser.Role = UpdatedUser.Role;
                    existingUser.Phone = UpdatedUser.Phone;
                    existingUser.Address = UpdatedUser.Address;

                    context.SaveChanges();

                    MessageBox.Show("Người dùng đã được cập nhật thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Quay lại UserManagementView
                    var mainWindow = Application.Current.MainWindow?.DataContext as MainViewModel;
                    if (mainWindow != null)
                    {
                        mainWindow.CurrentView = new UserManagementView { DataContext = new UserManagementViewModel() };
                        mainWindow.ImageVisibility = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật người dùng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
