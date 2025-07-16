using System;
using System.Linq;
using System.Windows.Controls;
using TrafficViolationFeedbackSystem.Data;

namespace TrafficViolationFeedbackSystem.Views.TrafficPolice
{
    public partial class ViewAllViolationsView : UserControl
    {
        private readonly TrafficViolationFeedbackSystemContext _context;
        public ViewAllViolationsView(TrafficViolationFeedbackSystemContext context)
        {
            InitializeComponent();
            _context = context;
            LoadAllViolations();
        }

        private void LoadAllViolations()
        {
            var violations = _context.Violations
                .Select(v => new
                {
                    ViolationId = v.ViolationId,
                    ViolationTypeName = v.Report.ViolationType.Name,
                    Description = v.Report.Description,
                    FineDate = v.FineDate,
                    FineAmount = v.FineAmount,
                    ViolatorName = v.Violator.FullName,
                    Status = v.Status
                })
                .ToList();
            dgAllViolations.ItemsSource = violations;
        }
    }
} 