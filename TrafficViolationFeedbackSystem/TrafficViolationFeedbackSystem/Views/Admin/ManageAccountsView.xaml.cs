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
using TrafficViolationFeedbackSystem.Data;

namespace TrafficViolationFeedbackSystem.Views.Admin
{
    /// <summary>
    /// Interaction logic for ManageAccountsView.xaml
    /// </summary>
    public partial class ManageAccountsView : UserControl
    {
        private readonly TrafficViolationFeedbackSystemContext _context;
        private readonly Action<UserControl> _setContent;

        public ManageAccountsView(Action<UserControl> setContent)
        {
            InitializeComponent();
            _context = new TrafficViolationFeedbackSystemContext();
            _setContent = setContent;
            LoadUsers();
        }
        private void LoadUsers(string filterName = null)
        {
            var users = _context.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filterName))
            {
                users = users.Where(u => u.FullName.ToLower().Contains(filterName.ToLower()));
            }
            var userList = users.ToList();
            UsersGrid.ItemsSource = userList;
            // Kiểm tra nếu danh sách rỗng và có bộ lọc
            if (!string.IsNullOrWhiteSpace(filterName) && userList.Count == 0)
            {
                MessageBox.Show("Không tìm thấy người dùng với tên đã nhập!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                // Làm mới danh sách về trạng thái ban đầu (hiển thị tất cả người dùng)
                UsersGrid.ItemsSource = _context.Users.ToList();
                //txtFilterName.Text = string.Empty; // Xóa nội dung TextBox lọc
            }
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            _setContent(new AddUserView( () => _setContent(new ManageAccountsView( _setContent))));
        }

        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is Models.User selectedUser)
            {
                _setContent(new EditUserView(selectedUser.UserId, () => _setContent(new ManageAccountsView(_setContent))));
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một tài khoản để sửa!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is Models.User selectedUser)
            {
                var result = MessageBox.Show($"Bạn có chắc muốn xóa tài khoản {selectedUser.FullName}?",
                    "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var user = _context.Users.Find(selectedUser.UserId);
                        if (user != null)
                        {
                            if (_context.Vehicles.Any(v => v.OwnerId == user.UserId) ||
                                _context.Reports.Any(r => r.ReporterId == user.UserId || r.ProcessedBy == user.UserId) ||
                                _context.Violations.Any(v => v.ViolatorId == user.UserId) ||
                                _context.Fines.Any(f => f.CreatedBy == user.UserId || f.ConfirmedBy == user.UserId) ||
                                _context.Appeals.Any(a => a.ViolatorId == user.UserId) ||
                                _context.Notifications.Any(n => n.UserId == user.UserId))
                            {
                                MessageBox.Show("Không thể xóa tài khoản này vì nó đang được sử dụng trong các bản ghi khác!",
                                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            _context.Users.Remove(user);
                            _context.SaveChanges();
                            LoadUsers();
                            MessageBox.Show("Xóa tài khoản thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một tài khoản để xóa!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFilterName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên người dùng để lọc!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            LoadUsers(txtFilterName.Text);
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers(null); 
            txtFilterName.Text = string.Empty; 
        }
    }
}
