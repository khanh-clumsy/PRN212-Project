using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TrafficViolationFeedbackSystem.Models;
using TrafficViolationFeedbackSystem;
using System.Windows.Input;

namespace TrafficViolationFeedbackSystem
{
    public class ViolationTypesViewModel : BaseViewModel
    {

        private ObservableCollection<ViolationType> _violationTypes;
        private ViolationType _selectedViolationType;

        public ObservableCollection<ViolationType> ViolationTypes
        {
            get => _violationTypes;
            set
            {
                _violationTypes = value;
                OnPropertyChanged(nameof(ViolationTypes));
            }
        }

        public ViolationType SelectedViolationType
        {
            get => _selectedViolationType;
            set
            {
                _selectedViolationType = value;
                OnPropertyChanged(nameof(SelectedViolationType));
                OnPropertyChanged(nameof(IsViolationTypeSelected));
            }
        }

        public bool IsViolationTypeSelected => SelectedViolationType != null;

        public ICommand CreateCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        private readonly Window _parentWindow;

        // Thêm thuộc tính IsCreateFormVisible


        public ViolationTypesViewModel(Window parentWindow)
        {
            _parentWindow = parentWindow;
            CreateCommand = new RelayCommand(Create);
            UpdateCommand = new RelayCommand(Update, CanUpdate);
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            LoadViolationTypes();
        }
        private void LoadViolationTypes()
        {
            try
            {
                using (var context = new TrafficViolationFeedbackSystemContext())
                {
                    ViolationTypes = new ObservableCollection<ViolationType>(
                        context.ViolationTypes
                            .Select(vt => new ViolationType
                            {
                                ViolationTypeId = vt.ViolationTypeId,
                                Name = vt.Name,
                                Description = vt.Description,
                                StandardFine = vt.StandardFine
                            })
                            .ToList()
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách loại vi phạm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        private void Create(object parameter)
        {
            var mainWindow = Application.Current.MainWindow?.DataContext as MainViewModel;
            if (mainWindow != null)
            {
                mainWindow.CurrentView = new ViolationTypeCreateView { DataContext = new ViolationTypeCreateViewModel() };
                mainWindow.ImageVisibility = false;
            }
        }

        private bool CanUpdate(object parameter) => SelectedViolationType != null;

        private void Update(object parameter)
        {
            if (SelectedViolationType == null) return;
            var mainWindow = Application.Current.MainWindow?.DataContext as MainViewModel;
            if (mainWindow != null)
            {
                mainWindow.CurrentView = new ViolationTypesUpdateView { DataContext = new ViolationTypeUpdateViewModel(SelectedViolationType) };
                mainWindow.ImageVisibility = false;
            }
        }

        private bool CanDelete(object parameter) => SelectedViolationType != null;

        private void Delete(object parameter)
        {
            if (SelectedViolationType == null) return;
            MessageBoxResult result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa loại vi phạm '{SelectedViolationType.Name}' (ID: {SelectedViolationType.ViolationTypeId})?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new TrafficViolationFeedbackSystemContext())
                    {
                        var violationTypeToDelete = context.ViolationTypes.FirstOrDefault(vt => vt.ViolationTypeId == SelectedViolationType.ViolationTypeId);
                        if (violationTypeToDelete != null)
                        {
                            context.ViolationTypes.Remove(violationTypeToDelete);
                            context.SaveChanges();
                            LoadViolationTypes();
                            SelectedViolationType = null;
                            MessageBox.Show("Loại vi phạm đã được xóa thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy loại vi phạm để xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa loại vi phạm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
