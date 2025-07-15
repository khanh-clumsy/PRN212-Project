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
using TrafficViolationFeedbackSystem.Services;
using Microsoft.EntityFrameworkCore;
using TrafficViolationFeedbackSystem.Data;

namespace TrafficViolationFeedbackSystem.Views.Authentication
{
    /// <summary>
    /// Interaction logic for ForgotPasswordView.xaml
    /// </summary>
    public partial class ForgotPasswordView : UserControl
    {
        public Action ShowLoginView { get; set; }
        private readonly TrafficViolationFeedbackSystemContext _context;
        private readonly EmailService _emailService = new EmailService();
        public Action<string> ShowVerifyCode { get; set; }
        public ForgotPasswordView(TrafficViolationFeedbackSystemContext context)
        {
            InitializeComponent();
            _context = context;
        }

        private void tblGoToLogin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowLoginView?.Invoke();
        }
        private void ResetUI()
        {
            btnSubmit.IsEnabled = true;
            txtStatus.Text = "";
        }
        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            btnSubmit.IsEnabled = false;
            txtStatus.Text = "Đang gửi mã xác nhận...";
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Vui lòng nhập email.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                ResetUI();
                return;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                MessageBox.Show("Email không tồn tại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                ResetUI();
                return;
            }

            //Chặn spam gửi mã xác nhận
            if (user.ResetCodeExpiry.HasValue && user.ResetCodeExpiry > DateTime.Now)
            {
                var secondsLeft = (user.ResetCodeExpiry.Value - DateTime.Now).TotalSeconds;
                if (secondsLeft > 240) // Nếu còn hơn 1 phút kể từ lần gửi gần nhất
                {
                    MessageBox.Show("Bạn đã yêu cầu mã gần đây. Vui lòng chờ một lúc rồi thử lại.", "Spam phát hiện", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ResetUI();
                    return;
                }
            }

            // Tạo mã xác nhận
            string resetCode = new Random().Next(100000, 999999).ToString();
            user.ResetCode = resetCode;
            user.ResetCodeExpiry = DateTime.Now.AddMinutes(5); // Hết hạn sau 5 phút

            await _context.SaveChangesAsync();

            // Gửi email
            await _emailService.SendEmailAsync(email, "Mã đặt lại mật khẩu", $"Mã xác nhận của bạn là: {resetCode}");

            MessageBox.Show("Mã xác nhận đã được gửi đến email của bạn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            ShowVerifyCode?.Invoke(email);
        }

        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSubmit_Click(sender, e);
            }
        }
    }
}
