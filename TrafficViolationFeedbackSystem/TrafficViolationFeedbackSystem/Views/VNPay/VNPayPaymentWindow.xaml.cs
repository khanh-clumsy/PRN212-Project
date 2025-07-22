using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
using TrafficViolationFeedbackSystem.Services;

namespace TrafficViolationFeedbackSystem.Views.VNPay
{
    /// <summary>
    /// Interaction logic for VNPayPaymentWindow.xaml
    /// </summary>
    public partial class VNPayPaymentWindow : Window
    {
        private readonly EmailService _emailService = new EmailService();
        private readonly TrafficViolationFeedbackSystemContext _context = new TrafficViolationFeedbackSystemContext();
        private bool _paymentHandled = false;
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
                        if (_paymentHandled) return;
                        var uri = new Uri(webView.Source.ToString());

                        if (!uri.Query.Contains("vnp_ResponseCode"))
                            return;

                        _paymentHandled = true; // Đặt flag

                        var query = HttpUtility.ParseQueryString(uri.Query);

                        string responseCode = query["vnp_ResponseCode"];
                        string txnRef = query["vnp_TxnRef"];

                        string[] parts = txnRef.Split('_');
                        if (parts.Length < 2)
                        {
                            MessageBox.Show("Mã giao dịch không hợp lệ.");
                            return;
                        }
                        string fineId = parts[0];
                        string transactionNo = query["vnp_TransactionNo"];
                        string amountRaw = query["vnp_Amount"];
                        this.Close();

                        if (responseCode == "00")
                        {
                            // Giao dịch thành công → gọi lưu DB nếu cần
                            await SavePaymentToDatabase(fineId, transactionNo, amountRaw);
                            await SendSuccessEmail(fineId, transactionNo, amountRaw);
                            MessageBox.Show("Thanh toán thành công!");
                        }
                        else if (!string.IsNullOrEmpty(responseCode))
                        {
                            MessageBox.Show("Thanh toán thất bại hoặc bị hủy!");
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
            string[] parts = fineIdRaw.Split('_');
            if (parts.Length < 1 || !int.TryParse(parts[0], out int fineId))
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
        private async Task SendSuccessEmail(string fineId, string transactionNo, string amountRaw)
        {
            try
            {
                string userId = AuthenticationContext.UserId;
                var user = _context.Users.FirstOrDefault(u => u.UserId == Int32.Parse(userId));
                if (user == null) { return; }
                string userEmail = user.Email;

                decimal amount = decimal.Parse(amountRaw) / 100;

                string subject = $"Xác nhận thanh toán thành công - Vi phạm #{fineId}";
                string body = $@"
            <h2>Chào bạn,</h2>
            <p>Thanh toán của bạn đã được xử lý thành công.</p>
            <ul>
                <li><strong>Mã vi phạm:</strong> {fineId}</li>
                <li><strong>Mã giao dịch:</strong> {transactionNo}</li>
                <li><strong>Số tiền:</strong> {amount:N0} VND</li>
            </ul>
            <p>Cảm ơn bạn đã sử dụng hệ thống phản hồi vi phạm giao thông.</p>
            <p><em>TrafficViolationFeedbackSystem</em></p>";

                await _emailService.SendEmailAsync(userEmail, subject, body);
            }
            catch (Exception ex)
            {
                MessageBox.Show("⚠ Gửi email thất bại: " + ex.Message);
            }
        }
    }
}
