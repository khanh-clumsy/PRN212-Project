using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using TrafficViolationFeedbackSystem.Models;

namespace TrafficViolationFeedbackSystem
{
    public class ViolationTypeUpdateViewModel : BaseViewModel
    {
        private ViolationType _updatedViolationType;

        public ViolationType UpdatedViolationType
        {
            get => _updatedViolationType;
            set
            {
                _updatedViolationType = value;
                OnPropertyChanged(nameof(UpdatedViolationType));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand UpdateCommand { get; }

        public ViolationTypeUpdateViewModel(ViolationType violationType)
        {
            UpdatedViolationType = violationType ?? new ViolationType();
            UpdateCommand = new RelayCommand(Update, CanUpdate);
        }

        private bool CanUpdate(object parameter)
        {
            return !string.IsNullOrWhiteSpace(UpdatedViolationType.Name) &&
                   UpdatedViolationType.StandardFine > 0;
        }

        private void Update(object parameter)
        {
            try
            {
                using (var context = new TrafficViolationFeedbackSystemContext())
                {
                    var existingViolationType = context.ViolationTypes.FirstOrDefault(vt => vt.ViolationTypeId == UpdatedViolationType.ViolationTypeId);
                    if (existingViolationType == null)
                    {
                        MessageBox.Show("Loại vi phạm không tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Kiểm tra tính duy nhất của Name (trừ bản ghi hiện tại)
                    if (context.ViolationTypes.Any(vt => vt.Name == UpdatedViolationType.Name && vt.ViolationTypeId != UpdatedViolationType.ViolationTypeId))
                    {
                        MessageBox.Show("Tên loại vi phạm đã tồn tại. Vui lòng chọn tên khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Kiểm tra StandardFine chỉ chứa số
                    if (!Regex.IsMatch(UpdatedViolationType.StandardFine.ToString(), @"^[0-9]+$"))
                    {
                        MessageBox.Show("Mức phạt tiêu chuẩn chỉ được chứa số.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Cập nhật thông tin
                    existingViolationType.Name = UpdatedViolationType.Name;
                    existingViolationType.Description = UpdatedViolationType.Description;
                    existingViolationType.StandardFine = UpdatedViolationType.StandardFine;

                    context.SaveChanges();

                    MessageBox.Show("Loại vi phạm đã được cập nhật thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Quay lại ViolationTypesView
                    var mainWindow = Application.Current.MainWindow?.DataContext as AdminDashboardModel;
                    if (mainWindow != null)
                    {
                        mainWindow.CurrentView = new ViolationTypesView { DataContext = new ViolationTypesViewModel(Application.Current.MainWindow) };
                        mainWindow.ImageVisibility = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật loại vi phạm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}