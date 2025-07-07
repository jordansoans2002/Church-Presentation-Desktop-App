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
    /// Interaction logic for SongLyrics.xaml
    /// </summary>
    public partial class SongLyrics : UserControl
    {
        public SongLyrics()
        {
            InitializeComponent();
        }

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender == textbox1)
            {
                textbox2.ScrollToVerticalOffset(e.VerticalOffset);
            }
            else
            {
                textbox1.ScrollToVerticalOffset(e.VerticalOffset);
            }
        }
    }
}
