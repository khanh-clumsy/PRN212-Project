using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TrafficViolationFeedbackSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace TrafficViolationFeedbackSystem
{
    public class ViolationStatisticsViewModel : BaseViewModel
    {

        private ObservableCollection<Violation> _violations;

        public ObservableCollection<Violation> Violations
        {
            get => _violations;
            set
            {
                _violations = value;
                OnPropertyChanged(nameof(Violations));
            }
        }

        public ViolationStatisticsViewModel()
        {
            LoadViolations();
        }

        private void LoadViolations()
        {
            try
            {
                using (var context = new TrafficViolationFeedbackSystemContext())
                {
                    Violations = new ObservableCollection<Violation>(
                        context.Violations
                            .Include(v => v.Report) 
                            .Include(v => v.Violator) 
                            .Where(v => v.Violator != null) 
                            .ToList() 
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading violations: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
