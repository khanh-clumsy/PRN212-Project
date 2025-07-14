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

namespace TrafficViolationFeedbackSystem.Views.Citizen

{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        private readonly TrafficViolationFeedbackSystemContext _context;
        private readonly Action<UserControl> setContent;

        public HomeView(Action<UserControl> setContent, TrafficViolationFeedbackSystemContext context)
        {
            InitializeComponent();
            _context = context;
            this.setContent = setContent;
        }
        private void btnReportViolation_Click(object sender, RoutedEventArgs e)
        {
            setContent?.Invoke(new ReportViolationView(_context));
        }

        private void btnViewReportList_Click(object sender, RoutedEventArgs e)
        {
            setContent?.Invoke(new ReportList());
        }

        private void btnAppeal_Click(object sender, RoutedEventArgs e)
        {
            setContent?.Invoke(new AppealView());
        }

        private void btnPayFine_Click(object sender, RoutedEventArgs e)
        {
            setContent?.Invoke(new PayFineView());
        }

        private void btnViewMyViolations_Click(object sender, RoutedEventArgs e)
        {
            setContent?.Invoke(new ViewViolationsView());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
