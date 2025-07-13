using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TrafficViolationFeedbackSystem.Models;

namespace TrafficViolationFeedbackSystem
{
    public class UserManagementViewModel : BaseViewModel
    {
        private ObservableCollection<User> _users;
        private User _selectedUser;
        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
                OnPropertyChanged(nameof(IsUserSelected)); 
            }
        }

        public bool IsUserSelected => SelectedUser != null;

        public ICommand CreateCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }

        public UserManagementViewModel()
        {
            CreateCommand = new RelayCommand(Create);
            UpdateCommand = new RelayCommand(Update, CanUpdate);
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                using (var context = new TrafficViolationFeedbackSystemContext())
                {
                    Users = new ObservableCollection<User>(
                        context.Users
                            .Select(u => new User
                            {
                                UserId = u.UserId,
                                Role = u.Role,
                                FullName = u.FullName,
                                Email = u.Email,
                                Password = u.Password,
                                Phone = u.Phone,
                                Address = u.Address
                            })
                            .ToList()
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errorr Load: {ex.Message}", "Errorr", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Create(object parameter)
        {
            // Điều hướng sang UserCreateView
            var mainWindow = Application.Current.MainWindow?.DataContext as MainViewModel;
            if (mainWindow != null)
            {
                mainWindow.CurrentView = new UserCreateView { DataContext = new UserCreateViewModel() };
                mainWindow.ImageVisibility = false;
            }
        }
        private bool CanUpdate(object parameter)
        {
            return SelectedUser != null;
        }

        private void Update(object parameter)
        {
            if (SelectedUser == null) return;

            var mainWindow = Application.Current.MainWindow?.DataContext as MainViewModel;
            if (mainWindow != null)
            {
                mainWindow.CurrentView = new UserUpdateView { DataContext = new UserUpdateViewModel(SelectedUser) };
                mainWindow.ImageVisibility = false;
            }
        }

        private bool CanDelete(object parameter)
        {
            return SelectedUser != null;
        }

        private void Delete(object parameter)
        {
            if (SelectedUser == null) return;

            // Hiển thị thông báo xác nhận
            MessageBoxResult result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa người dùng với ID {SelectedUser.UserId} - {SelectedUser.FullName}?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new TrafficViolationFeedbackSystemContext())
                    {
                        var userToDelete = context.Users.FirstOrDefault(u => u.UserId == SelectedUser.UserId);
                        if (userToDelete != null)
                        {
                            context.Users.Remove(userToDelete);
                            context.SaveChanges();

                            // Làm mới danh sách Users
                            LoadUsers();
                            SelectedUser = null; // Xóa lựa chọn sau khi xóa
                            MessageBox.Show("Người dùng đã được xóa thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy người dùng để xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa người dùng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
