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

        public ForgotPasswordView(TrafficViolationFeedbackSystemContext context)
        {
            InitializeComponent();
            _context = context;
        }

        private void tblGoToLogin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShowLoginView?.Invoke();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
