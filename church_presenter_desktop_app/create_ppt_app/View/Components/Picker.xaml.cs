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

namespace create_ppt_app.View.Components
{
    /// <summary>
    /// Interaction logic for Picker.xaml
    /// </summary>
    public partial class Picker : UserControl
    {
        public Picker()
        {
            InitializeComponent();
        }


        public string SettingValue
        {
            get { return (string)GetValue(SettingValueProperty); }
            set { SetValue(SettingValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SettingValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SettingValueProperty =
            DependencyProperty.Register(nameof(SettingValue), typeof(string), typeof(Picker), new PropertyMetadata(string.Empty));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(Picker), new PropertyMetadata(null));




        //public static readonly RoutedEvent PickerClickEvent = 
        //    EventManager.RegisterRoutedEvent(nameof(PickerCommand), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Picker));
        //public event RoutedEventHandler PickerCommand
        //{
        //    add { AddHandler(PickerClickEvent, value); }
        //    remove { RemoveHandler(PickerClickEvent, value); }
        //}

        //private void OnPickerClick(object sender, RoutedEventArgs e)
        //{
        //    RaiseEvent(new RoutedEventArgs(PickerClickEvent));
        //}
    }
}
