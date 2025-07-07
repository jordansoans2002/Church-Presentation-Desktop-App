using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class TextInput : UserControl, INotifyPropertyChanged, IDataErrorInfo
    {
        public TextInput()
        {
            InitializeComponent();
        }



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextInput), new PropertyMetadata());



        public Func<string,string?> ValidateInput
        {
            get { return (Func<string,string?>)GetValue(ValidateInputProperty); }
            set { SetValue(ValidateInputProperty, value); }
        }

        public static readonly DependencyProperty ValidateInputProperty =
            DependencyProperty.Register(nameof(ValidateInput), typeof(Func<string,string?>), typeof(TextInput), new PropertyMetadata());



        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TextInput;
            control?.Validate();
        }

        private void Validate()
        {
            ErrorMessage = ValidateInput?.Invoke(Text);
            HasError = !string.IsNullOrEmpty(ErrorMessage);
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(HasError));
        }

        public string Error => null;
        public string this[string columnName]
        {
            get
           {
                if(columnName == nameof(Text))
                {
                    return ValidateInput?.Invoke(Text);
                }
                return null;
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        private bool _hasError;
        public bool HasError
        {
            get => _hasError;
            set
            {
                _hasError = value;
                OnPropertyChanged(nameof(HasError));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void LostFocus(object sender, RoutedEventArgs e)
        {
            //if (sender is ComboBox cmb)
            //{
            //    cmb.Text = Text;
            //}
        }
    }
}
