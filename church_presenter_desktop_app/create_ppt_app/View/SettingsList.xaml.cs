using create_ppt_app.Model;
using create_ppt_app.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace create_ppt_app.View
{
    /// <summary>
    /// Interaction logic for SettingsList.xaml
    /// </summary>
    public partial class SettingsList : UserControl
    {
        //public List<SettingModel> Settings { get; set; }
        public SettingsList()
        {
            InitializeComponent();

        }

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender == settingNames)
            {
                var scrollViewer = GetScrollViewer(settingValues);
                scrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
            }
            else
            {
                var scrollViewer = GetScrollViewer(settingNames);
                scrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
            }
        }
        private ScrollViewer GetScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer) return (ScrollViewer)depObj;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                var result = GetScrollViewer(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}
