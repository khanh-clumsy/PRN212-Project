using System;
using System.Windows.Controls;
using TrafficViolationFeedbackSystem.Data;

namespace TrafficViolationFeedbackSystem.Views.TrafficPolice
{
    public partial class TrafficPoliceHomeView : UserControl
    {
        private readonly Action<UserControl> setContent;
        private readonly TrafficViolationFeedbackSystemContext _context; 
        public TrafficPoliceHomeView(Action<UserControl> setContent, TrafficViolationFeedbackSystemContext context)
        {
            InitializeComponent();
            _context = context;
            this.setContent = setContent;
        }
        private void btnViewAllReports_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            setContent?.Invoke(new ViewAllReportsView(_context));
        }
        private void btnViewAppeals_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            setContent?.Invoke(new ViewAppealsView(_context));
        }
        private void btnAddVehicle_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            setContent?.Invoke(new AddVehicleView(_context));
        }
        private void btnViewVehicles_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            setContent?.Invoke(new VehicleListView(_context));
        }
    }
} 