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
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace TrafficViolationFeedbackSystem.Views.Authentication
{
    /// <summary>
    /// Interaction logic for VerifyCodeView.xaml
    /// </summary>
    public partial class VerifyCodeView : UserControl
    {
        private readonly string _email;
        public Action ShowLoginView { get; set; }
        public Action<string> ShowResetPasswordView { get; set; }
        private readonly TrafficViolationFeedbackSystemContext _context = new TrafficViolationFeedbackSystemContext();
        public VerifyCodeView(string email)
        {
            InitializeComponent();
            this._email = email;
        }

        private async void btnVerify_Click(object sender, RoutedEventArgs e)
        {
            string code = txtCode.Text.Trim();
            if (string.IsNullOrWhiteSpace(_email) || string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ email và mã xác nhận.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == _email);
            if (user == null)
            {
                MessageBox.Show("Email không tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (user.ResetCode != code)
            {
                MessageBox.Show("Mã xác nhận không đúng.", "Sai mã", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!user.ResetCodeExpiry.HasValue || user.ResetCodeExpiry < DateTime.Now)
            {
                MessageBox.Show("Mã xác nhận đã hết hạn.", "Hết hạn", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBox.Show("Mã xác nhận đúng, mời bạn thay đổi mật khẩu.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

            //  Nếu đúng mã và còn hiệu lực → chuyển sang bước đặt lại mật khẩu
            ShowResetPasswordView?.Invoke(_email);
        }
        private void tblGoToLogin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowLoginView?.Invoke();
        }

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnVerify_Click(sender, e);
            }
        }
    }
}
