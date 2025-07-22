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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrafficViolationFeedbackSystem.Controllers;
using TrafficViolationFeedbackSystem.Models;
using TrafficViolationFeedbackSystem.ViewModels;

namespace TrafficViolationFeedbackSystem.Views.Citizen
{
    /// <summary>
    /// Interaction logic for AllFineView.xaml
    /// </summary>
    public partial class AllFineView : UserControl
    {
        private readonly FineController _fineController;
        private List<FineViewModel> _allFines;

        public AllFineView()
        {
            _fineController = new FineController();
            InitializeComponent();
            LoadFine();
        }

        public void LoadFine()
        {
            _allFines = _fineController.GetAllByCurrentUser();
            dgFines.ItemsSource = _allFines;
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string keyword = txtSearch.Text?.Trim().ToLower();
            string selectedStatus = (cbStatus.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string mappedStatus = selectedStatus switch
            {
                "Đã thanh toán" => "Success",
                "Chưa thanh toán" => "Pending",
                "Thất bại" => "Failed",
                _ => null // Tất cả hoặc null
            };
            var filtered = _allFines.Where(f =>
                (string.IsNullOrEmpty(keyword) || f.ViolationName?.ToLower().Contains(keyword) == true) &&
                (mappedStatus == null || f.Status.Equals(mappedStatus, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            dgFines.ItemsSource = filtered;
        }
    }
}
