
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using TrafficViolationFeedbackSystem.Models;

namespace TrafficViolationFeedbackSystem
{
    public class AuditLogViewModel : BaseViewModel
    {
        private ObservableCollection<AuditLog> _auditLogs;

        public ObservableCollection<AuditLog> AuditLogs
        {
            get => _auditLogs;
            set
            {
                _auditLogs = value;
                OnPropertyChanged(nameof(AuditLogs));
            }
        }

        public AuditLogViewModel()
        {
            LoadAuditLogs();
        }

        private void LoadAuditLogs()
        {
            try
            {
                using (var context = new TrafficViolationFeedbackSystemContext())
                {
                    AuditLogs = new ObservableCollection<AuditLog>(
                        context.AuditLogs
                            .AsNoTracking() // Tối ưu hiệu suất
                            .Include(log => log.User) // Lấy User.FullName
                            .Where(log => log.User != null) // Đảm bảo User không null
                            .OrderByDescending(log => log.Timestamp) // Sắp xếp theo Timestamp giảm dần
                            .ToList()
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading audit logs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
