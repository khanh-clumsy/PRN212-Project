
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Models;
using TrafficViolationFeedbackSystem.Views.Admin;

namespace TrafficViolationFeedbackSystem
{
    public class UserManagementViewModel : BaseViewModel
    {
        private ObservableCollection<User> _allUsers;
        private ObservableCollection<User> _users;
        private User _selectedUser;
        private string _filterFullName;

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

        public string FilterFullName
        {
            get => _filterFullName;
            set
            {
                _filterFullName = value;
                OnPropertyChanged(nameof(FilterFullName));
            }
        }

        public ICommand CreateCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand ResetFilterCommand { get; }

        public UserManagementViewModel()
        {
            CreateCommand = new RelayCommand(Create);
            UpdateCommand = new RelayCommand(Update, CanUpdate);
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            FilterCommand = new RelayCommand(Filter, CanFilter);
            ResetFilterCommand = new RelayCommand(ResetFilter, CanResetFilter);
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                using (var context = new TrafficViolationFeedbackSystemContext())
                {
                    _allUsers = new ObservableCollection<User>(
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
                    Users = new ObservableCollection<User>(_allUsers);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Load: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Create(object parameter)
        {
            var mainWindow = Application.Current.MainWindow?.DataContext as AdminDashboardModel;
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

            var mainWindow = Application.Current.MainWindow?.DataContext as AdminDashboardModel;
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

                            LoadUsers();
                            SelectedUser = null;
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

        private bool CanFilter(object parameter)
        {
            return true;
        }

        private void Filter(object parameter)
        {
            try
            {
                if (_allUsers == null || !_allUsers.Any())
                {
                    MessageBox.Show("No data available to filter.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(FilterFullName))
                {
                    MessageBox.Show("Bạn chưa nhập tên người dùng.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var filtered = _allUsers
                    .Where(u => u.FullName != null && u.FullName.Contains(FilterFullName ?? "", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (filtered.Any())
                {
                    Users = new ObservableCollection<User>(filtered);
                }
                else
                {
                    Users = new ObservableCollection<User>();
                    MessageBox.Show("No users found for the given name.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanResetFilter(object parameter)
        {
            return true;
        }

        private void ResetFilter(object parameter)
        {
            FilterFullName = string.Empty;
            Users = new ObservableCollection<User>(_allUsers ?? new ObservableCollection<User>());
        }
    }
}
