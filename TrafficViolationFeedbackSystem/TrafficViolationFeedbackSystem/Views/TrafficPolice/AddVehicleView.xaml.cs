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
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string plateNumber = PlateBox.Text.Trim();
            string brand = BrandBox.Text.Trim();
            string model = ModelBox.Text.Trim();
            string ownerEmail = OwnerEmailBox.Text.Trim();
            string yearText = YearBox.Text.Trim();

            if (string.IsNullOrEmpty(plateNumber) ||
                string.IsNullOrEmpty(brand) ||
                string.IsNullOrEmpty(model) ||
                string.IsNullOrEmpty(ownerEmail) ||
                string.IsNullOrEmpty(yearText))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return;
            }

            // Kiểm tra năm sản xuất
            if (!int.TryParse(yearText, out int year))
            {
                MessageBox.Show("Năm sản xuất không hợp lệ!");
                return;
            }

            // Kiểm tra biển số đã tồn tại chưa
            var existingVehicle = _context.Vehicles.FirstOrDefault(v => v.PlateNumber == plateNumber);
            if (existingVehicle != null)
            {
                MessageBox.Show("Biển số đã tồn tại trong hệ thống!");
                return;
            }

            // Kiểm tra người dùng có tồn tại không
            var user = _context.Users.FirstOrDefault(u => u.Email == ownerEmail);
            if (user == null)
            {
                MessageBox.Show("Không tìm thấy người dùng với email này.");
                return;
            }

            // Tạo mới phương tiện
            var vehicle = new Vehicle
            {
                PlateNumber = plateNumber,
                Brand = brand,
                Model = model,
                ManufactureYear = year,
                OwnerId = user.UserId
            };

            // Thêm vào DB
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
