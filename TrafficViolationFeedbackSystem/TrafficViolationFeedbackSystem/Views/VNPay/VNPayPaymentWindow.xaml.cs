using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TrafficViolationFeedbackSystem.Data;

namespace TrafficViolationFeedbackSystem.Views.VNPay
{
    /// <summary>
    /// Interaction logic for VNPayPaymentWindow.xaml
    /// </summary>
    public partial class VNPayPaymentWindow : Window
    {
        public VNPayPaymentWindow(string paymentUrl)
        {
            InitializeComponent();

            Loaded += async (s, e) =>
            {
                await webView.EnsureCoreWebView2Async();
                webView.CoreWebView2.Navigate(paymentUrl);

                webView.CoreWebView2.NavigationCompleted += async (s2, e2) =>
                {
                    try
                    {
                        var uri = new Uri(webView.Source.ToString());
                        var query = HttpUtility.ParseQueryString(uri.Query);

                        string responseCode = query["vnp_ResponseCode"];
                        string fineId = query["vnp_TxnRef"];
                        string transactionNo = query["vnp_TransactionNo"];
                        string amountRaw = query["vnp_Amount"];

                        if (responseCode == "00")
                        {
                            // ✅ Giao dịch thành công → gọi lưu DB nếu cần
                            await SavePaymentToDatabase(fineId, transactionNo, amountRaw);

                            MessageBox.Show("✅ Thanh toán thành công!");
                            this.Close();
                        }
                        else if (!string.IsNullOrEmpty(responseCode))
                        {
                            MessageBox.Show("❌ Thanh toán thất bại hoặc bị hủy!");
                            this.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi xử lý kết quả thanh toán: " + ex.Message);
                        this.Close();
                    }
                };
            };
        }
        private async Task SavePaymentToDatabase(string fineIdRaw, string transactionCode, string amountRaw)
        {
            if (!int.TryParse(fineIdRaw, out int fineId))
            {
                MessageBox.Show("Mã hóa đơn không hợp lệ.");
                return;
            }

            decimal amount = decimal.TryParse(amountRaw, out var raw) ? raw / 100m : 0;

            using var context = new TrafficViolationFeedbackSystemContext();

            var fine = await context.Fines.FindAsync(fineId);
            if (fine == null)
            {
                MessageBox.Show("Không tìm thấy hóa đơn cần cập nhật.");
                return;
            }

            // Cập nhật Fine
            fine.TransactionCode = transactionCode;
            fine.Status = "Success";
            fine.PaymentMethod = "VNPay";
            fine.PaymentDate = DateTime.Now;
            fine.Amount = amount;

            // Cập nhật trạng thái Violation
            var violation = await context.Violations.FindAsync(fine.ViolationId);
            if (violation != null && violation.Status != "Paid")
            {
                violation.Status = "Paid";
            }

            await context.SaveChangesAsync();
        }

    }
}
