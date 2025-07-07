using create_ppt_app.Model;
using create_ppt_app.ViewModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace create_ppt_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Handles the input, dropdown suggestions and selection for the title input textbox
    /// Handles reordering of songs in song list
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel viewModel;

        public MainWindow(MainWindowViewModel vm)
        {
            InitializeComponent();
            viewModel = vm;
        }

        private void SongList_MouseMove(object sender, MouseEventArgs e)
        {
            if(sender is FrameworkElement frameworkElement && e.LeftButton == MouseButtonState.Pressed)
            {
                Debug.Print(frameworkElement.DataContext.ToString());
                SongDetailsViewModel song = (SongDetailsViewModel)frameworkElement.DataContext;
                
                viewModel.isMoving = true;
                DragDropEffects dragDropResult = DragDrop.DoDragDrop(
                    frameworkElement,
                    new DataObject(DataFormats.Serializable, song),
                    DragDropEffects.Move
                );
                viewModel.isMoving = false;
            }
        }

        private void SongList_DragOver(object sender, DragEventArgs e)
        {
            if(sender is FrameworkElement element)
            {
                SongDetailsViewModel insertSong = (SongDetailsViewModel)e.Data.GetData(DataFormats.Serializable);
                SongDetailsViewModel targetSong =  (SongDetailsViewModel)element.DataContext;
                if (insertSong == targetSong || targetSong == null)
                    return;
                int oldPos = viewModel.SongDetails.IndexOf(insertSong);
                int newPos = viewModel.SongDetails.IndexOf(targetSong);
                if(oldPos != -1 && newPos != -1 && oldPos != newPos)
                {
                    viewModel.SongDetails.Move(oldPos, newPos);
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ApplicationSettingsViewModel.Instance.Save();
        }


        private static T GetChildOfType<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}