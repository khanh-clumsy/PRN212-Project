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
using Microsoft.EntityFrameworkCore;
using TrafficViolationFeedbackSystem.Data;
using TrafficViolationFeedbackSystem.Services;
using TrafficViolationFeedbackSystem.ViewModels;
using TrafficViolationFeedbackSystem.Views.VNPay;

namespace TrafficViolationFeedbackSystem.Views.Citizen
{
    /// <summary>
    /// Interaction logic for PayFineView.xaml
    /// </summary>
    public partial class PayFineView : UserControl
    {
        private readonly TrafficViolationFeedbackSystemContext _context;
        public PayFineView()
        {
            InitializeComponent();
            _context = new TrafficViolationFeedbackSystemContext();
            LoadFine();
        }

        public void LoadFine()
        {
            int currentUserId = int.Parse(AuthenticationContext.UserId);

            var fines = _context.Fines
                .Include(f => f.Violation)
                .Where(f => f.Violation.ViolatorId == currentUserId && f.Status == "Pending")
                .Select(f => new FineViewModel
                {
                    FineID = f.FineId,
                    ViolationID = f.ViolationId,
                    Amount = f.Amount,
                    Status = f.Status,
                    PaymentDate = f.PaymentDate
                })
                .ToList();
            dgFines.ItemsSource = fines;
        }
        private void BtnPay_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is FineViewModel fine)
            {
                string paymentUrl = VNPayHelper.CreatePaymentUrl(fine.Amount ?? 0, fine.FineID.ToString(), $"Thanh toán vi phạm {fine.FineID}");
                var payWindow = new VNPayPaymentWindow(paymentUrl);
                payWindow.ShowDialog();
            }
        }

    }
}
