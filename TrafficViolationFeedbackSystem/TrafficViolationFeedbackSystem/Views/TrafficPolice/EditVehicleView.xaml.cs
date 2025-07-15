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
using TrafficViolationFeedbackSystem.DAO;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Models;

namespace TrafficViolationFeedbackSystem.Views.TrafficPolice
{
    public partial class EditVehicleView : Window
    {
        private readonly TrafficViolationFeedbackSystemContext _context;
        private readonly Vehicle _vehicle;

        public EditVehicleView(TrafficViolationFeedbackSystemContext context, Vehicle vehicleToEdit)
        {
            InitializeComponent();
            _context = context;
            _vehicle = vehicleToEdit;
            LoadVehicle();
        }

        private void LoadVehicle()
        {
            PlateBox.Text = _vehicle.PlateNumber;
            BrandBox.Text = _vehicle.Brand;
            ModelBox.Text = _vehicle.Model;
            YearBox.Text = _vehicle.ManufactureYear.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(YearBox.Text.Trim(), out int year))
            {
                MessageBox.Show("Năm sản xuất không hợp lệ!");
                return;
            }

            _vehicle.PlateNumber = PlateBox.Text.Trim();
            _vehicle.Brand = BrandBox.Text.Trim();
            _vehicle.Model = ModelBox.Text.Trim();
            _vehicle.ManufactureYear = year;

            var dao = new VehicleDAO();
            dao.UpdateVehicle(_vehicle);

            MessageBox.Show("Cập nhật thành công!");
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}