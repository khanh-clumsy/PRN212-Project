using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using TrafficViolationFeedbackSystem.Models;

namespace TrafficViolationFeedbackSystem
{
    public class ViolationTypeCreateViewModel : BaseViewModel
    {
        private ViolationType _newViolationType = new ViolationType();

        public ViolationType NewViolationType
        {
            get => _newViolationType;
            set
            {
                _newViolationType = value;
                OnPropertyChanged(nameof(NewViolationType));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand SubmitCommand { get; }

        public ViolationTypeCreateViewModel()
        {
            SubmitCommand = new RelayCommand(Submit, CanSubmit);
        }

        private bool CanSubmit(object parameter)
        {
            return !string.IsNullOrWhiteSpace(NewViolationType.Name);
               // && NewViolationType.StandardFine > 0;
        }

        private void Submit(object parameter)
        {
            try
            {
                using (var context = new TrafficViolationFeedbackSystemContext())
                {
                    if (context.ViolationTypes.Any(vt => vt.Name == NewViolationType.Name))
                    {
                        MessageBox.Show("Tên loại vi phạm đã tồn tại. Vui lòng chọn tên khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Kiểm tra StandardFine chỉ chứa số
                    if (!Regex.IsMatch(NewViolationType.StandardFine.ToString(), @"^[0-9]+$"))
                    {
                        MessageBox.Show("Mức phạt tiêu chuẩn chỉ được chứa số.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (NewViolationType.StandardFine <= 0)
                    {
                        MessageBox.Show("Mức phạt tiêu chuẩn phải lớn hơn 0 VND.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var violationType = new ViolationType
                    {
                        Name = NewViolationType.Name,
                        Description = NewViolationType.Description,
                        StandardFine = NewViolationType.StandardFine
                    };
                    context.ViolationTypes.Add(violationType);
                    context.SaveChanges();

                    MessageBox.Show("Loại vi phạm đã được tạo thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

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
                MessageBox.Show($"Lỗi khi tạo loại vi phạm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}