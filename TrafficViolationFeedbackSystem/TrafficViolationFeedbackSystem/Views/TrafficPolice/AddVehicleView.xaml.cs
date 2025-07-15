using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Models;
using TrafficViolationFeedbackSystem.DAO;

namespace TrafficViolationFeedbackSystem.Views.TrafficPolice
{
    public partial class AddVehicleView : UserControl
    {
        private readonly TrafficViolationFeedbackSystemContext _context;

        public AddVehicleView(TrafficViolationFeedbackSystemContext context)
        {
            InitializeComponent();
            _context = context;
            LoadUsers();
        }

        private void LoadUsers()
        {
            var users = _context.Users.ToList();
            OwnerComboBox.ItemsSource = users;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(YearBox.Text.Trim(), out int year))
            {
                MessageBox.Show("Năm sản xuất không hợp lệ!");
                return;
            }

            if (OwnerComboBox.SelectedItem is not TrafficViolationFeedbackSystem.Models.User selectedUser)
            {
                MessageBox.Show("Vui lòng chọn chủ sở hữu.");
                return;
            }

            var vehicle = new Vehicle
            {
                PlateNumber = PlateBox.Text.Trim(),
                Brand = BrandBox.Text.Trim(),
                Model = ModelBox.Text.Trim(),
                ManufactureYear = year,
                OwnerId = selectedUser.UserId
            };

            var dao = new VehicleDAO();
            dao.InsertVehicle(vehicle);

            MessageBox.Show("Thêm phương tiện thành công!");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new VehicleListView(_context);
        }


    }
}
