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
using TrafficViolationFeedbackSystem.DAO;
using TrafficViolationFeedbackSystem.Services;

namespace TrafficViolationFeedbackSystem.Views.Citizen
{
    /// <summary>
    /// Interaction logic for ViewViolationsView.xaml
    /// </summary>
    public partial class ViewViolationsView : UserControl
    {
        private int userId = Int32.Parse(AuthenticationContext.UserId);
        public ViewViolationsView()
        {
            InitializeComponent();
            loadViolationView();
        }

        void loadViolationView()
        {
            ViolationDAO dao = new ViolationDAO();
            var violation = dao.GetViolationsForUser(userId);
            this.dgViolations.ItemsSource = violation;
        }
        private void AppealButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int violationId)
            {
                var win = new SendAppealWindow(violationId, userId);
                win.Owner = Window.GetWindow(this);
                win.ShowDialog();
            }
        }

    }
}
