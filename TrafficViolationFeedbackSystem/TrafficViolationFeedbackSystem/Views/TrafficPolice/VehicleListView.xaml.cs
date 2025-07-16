using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using System.Windows.Navigation;
using TrafficViolationFeedbackSystem.DAO;

namespace TrafficViolationFeedbackSystem.Views.TrafficPolice
{
    public partial class VehicleListView : UserControl
    {
        private readonly Data.TrafficViolationFeedbackSystemContext _context;

        public VehicleListView(Data.TrafficViolationFeedbackSystemContext context)
        {
            InitializeComponent();
            _context = context;
            LoadAllVehicles();
        }

        private void LoadAllVehicles()
        {
            var dao = new VehicleDAO();
            var vehicles = dao.GetAllVehiclesWithUsers();
            VehicleDataGrid.ItemsSource = vehicles;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string plateKeyword = PlateSearchBox.Text.Trim();

            var dao = new VehicleDAO();
            var result = dao.SearchVehicles(plateKeyword);

            VehicleDataGrid.ItemsSource = result;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int vehicleId)
            {
                var dao = new VehicleDAO();
                var vehicle = dao.GetVehicleById(vehicleId);
                if (vehicle != null)
                {
                    var editPopup = new EditVehicleView(_context, vehicle)
                    {
                        Owner = Application.Current.MainWindow
                    };

                    if (editPopup.ShowDialog() == true)
                    {
                        LoadAllVehicles(); // Refresh lại grid sau khi sửa
                    }
                }
            }
        }



        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int vehicleId)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa phương tiện này?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var dao = new VehicleDAO();
                    dao.DeleteVehicle(vehicleId);
                    LoadAllVehicles();
                }
            }
            else
            {
                MessageBox.Show("Không lấy được ID từ Tag. Tag: " + (button?.Tag?.ToString() ?? "null"));
            }
        }


    }
}