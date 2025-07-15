
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TrafficViolationFeedbackSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace TrafficViolationFeedbackSystem
{
    public class ViolationStatisticsViewModel : BaseViewModel
    {
        private ObservableCollection<Violation> _allViolations;
        private ObservableCollection<Violation> _violations;
        private string _filterPlateNumber;

        public ObservableCollection<Violation> Violations
        {
            get => _violations;
            set
            {
                _violations = value;
                OnPropertyChanged(nameof(Violations));
            }
        }

        public string FilterPlateNumber
        {
            get => _filterPlateNumber;
            set
            {
                _filterPlateNumber = value;
                OnPropertyChanged(nameof(FilterPlateNumber));
            }
        }

        public ICommand FilterCommand { get; }
        public ICommand ResetFilterCommand { get; }

        public ViolationStatisticsViewModel()
        {
            LoadAllViolations();
            FilterCommand = new RelayCommand(Filter, CanFilter);
            ResetFilterCommand = new RelayCommand(ResetFilter, CanResetFilter);
        }

        private void LoadAllViolations()
        {
            try
            {
                using (var context = new TrafficViolationFeedbackSystemContext())
                {
                    _allViolations = new ObservableCollection<Violation>(
                        context.Violations
                            .Include(v => v.Report)
                            .Include(v => v.Violator)
                            .Where(v => v.Violator != null && v.Report != null)
                            .ToList()
                    );
                    Violations = new ObservableCollection<Violation>(_allViolations);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading violations: {ex.Message}\nStackTrace: {ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanFilter(object parameter)
        {
            return true;
        }

        private void Filter(object parameter)
        {
            try
            {
                if (_allViolations == null || !_allViolations.Any())
                {
                    MessageBox.Show("No data available to filter.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(FilterPlateNumber))
                {
                    MessageBox.Show("Bạn chưa nhập biển số.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var filtered = _allViolations
                    .Where(v => v.Report != null && v.Report.PlateNumber != null &&
                                v.Violator != null && v.Violator.FullName != null &&
                                v.Report.PlateNumber.Contains(FilterPlateNumber ?? "", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (filtered.Any())
                {
                    Violations = new ObservableCollection<Violation>(filtered);
                }
                else
                {
                    Violations = new ObservableCollection<Violation>();
                    MessageBox.Show("Không có vi phạm nào được tìm thấy theo biển số .", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering violations: {ex.Message}\nStackTrace: {ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanResetFilter(object parameter)
        {
            return true;
        }

        private void ResetFilter(object parameter)
        {
            FilterPlateNumber = string.Empty;
            Violations = new ObservableCollection<Violation>(_allViolations ?? new ObservableCollection<Violation>());
        }
    }
}
