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
using System.Windows.Shapes;
using System;
using System.IO;


namespace TrafficViolationFeedbackSystem.Views.User
{
    public partial class AttachmentPreviewWindow : Window
    {
        public AttachmentPreviewWindow(string fileName)
        {
            InitializeComponent();

            string fullPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "Image", "Report", fileName);

            if (!File.Exists(fullPath))
            {
                MessageBox.Show("File không tồn tại: " + fullPath, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            string ext = System.IO.Path.GetExtension(fileName).ToLower();

            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
            {
                var img = new Image
                {
                    Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(fullPath)),
                    Stretch = Stretch.Uniform
                };
                contentHolder.Content = img;
            }
            else if (ext == ".mp4")
            {
                var media = new MediaElement
                {
                    Source = new Uri(fullPath),
                    LoadedBehavior = MediaState.Play,
                    UnloadedBehavior = MediaState.Stop
                };
                contentHolder.Content = media;
            }
            else
            {
                MessageBox.Show("Không hỗ trợ định dạng này.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Close();
            }
        }
    }
}
