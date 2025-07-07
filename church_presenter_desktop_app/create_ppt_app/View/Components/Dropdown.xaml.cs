using create_ppt_app.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interaction logic for Dropdown.xaml
    /// </summary>
    public partial class Dropdown : UserControl
    {
        public Dropdown()
        {
            InitializeComponent();
        }

        private string inputText = "";
        private int caretIndex = 0;
        public string BorderColor { get; set; } = "Black";

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(Dropdown), new PropertyMetadata());


        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(Dropdown), new PropertyMetadata());

        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register(nameof(IsEditable), typeof(bool), typeof(Dropdown), new PropertyMetadata());

        public IEnumerable<string> ItemsSource
        {
            get { return (IEnumerable<string>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<string>), typeof(Dropdown), new PropertyMetadata());

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(Dropdown), new PropertyMetadata(null));

        public Func<string,string> ValidateInput
        {
            get { return (Func<string,string>)GetValue(ValidateInputProperty); }
            set { SetValue(ValidateInputProperty, value); }
        }
        public static readonly DependencyProperty ValidateInputProperty =
            DependencyProperty.Register("ValidateInput", typeof(Func<string,string>), typeof(Dropdown), new PropertyMetadata());

        public Action<string>? UpdateOptions
        {
            get { return (Action<string>)GetValue(UpdateOptionsProperty); }
            set { SetValue(UpdateOptionsProperty, value); }
        }
        public static readonly DependencyProperty UpdateOptionsProperty =
            DependencyProperty.Register(nameof(UpdateOptions), typeof(Action<string>), typeof(Dropdown), new PropertyMetadata());

        public Action<string,string>? OnOptionSelected
        {
            get { return (Action<string,string>)GetValue(OnOptionSelectedProperty); }
            set { SetValue(OnOptionSelectedProperty, value); }
        }


        public static readonly DependencyProperty OnOptionSelectedProperty =
            DependencyProperty.Register(nameof(OnOptionSelected), typeof(Action<string,string>), typeof(Dropdown), new PropertyMetadata());

        private void DropdownInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is ComboBox cmb)
            {
                TextBox? textbox = cmb.Template.FindName("PART_EditableTextBox", cmb) as TextBox;
                if (textbox == null) return;

                switch (e.Key)
                {
                    case Key.Up:
                    case Key.Down:
                        if (e.Key == Key.Up)
                        {

                            if (cmb.SelectedIndex <= -1)
                                cmb.SelectedIndex = cmb.Items.Count - 1;
                            else
                                cmb.SelectedIndex -= 1;
                        }
                        else
                        {
                            if (cmb.SelectedIndex >= cmb.Items.Count - 1)
                                cmb.SelectedIndex = -1;
                            else
                                cmb.SelectedIndex += 1;
                        }

                        if (inputText != null)
                        {
                            textbox.Text = inputText;
                            SetCaret(textbox);
                        }
                        e.Handled = true;
                        break;

                    default:
                        break;
                }
            }
        }


        private void DropdownInput_KeyUp(object sender, KeyEventArgs e)
        {
            if(sender is ComboBox cmb)
            {
                TextBox? textbox = cmb.Template.FindName("PART_EditableTextBox", cmb) as TextBox;
                if (textbox == null) return;
            
                caretIndex = textbox.CaretIndex;
                switch (e.Key)
                {
                    case Key.Up:
                    case Key.Down:
                        textbox.Text = inputText;
                        SetCaret(textbox, textbox.Text.Length);
                        break;
                    case Key.Enter:
                        cmb.IsDropDownOpen = false;
                        if (cmb.SelectedItem != null)
                        {
                            string t = cmb.SelectedItem.ToString();
                            textbox.Text = t;
                            SetCaret(textbox);
                            OnOptionSelected?.Invoke(t,inputText);
                            inputText = t;
                        }
                        else
                        {
                            inputText = textbox.Text;
                            if(IsReadOnly is true && cmb.Items.Contains(inputText))
                            {
                                break;
                            }
                            OnOptionSelected?.Invoke("",inputText);
                        }
                        e.Handled = true;
                        break;

                    case Key.Escape:
                        cmb.IsDropDownOpen = false;
                        e.Handled = true;
                        break;

                    default:
                        inputText = textbox.Text;
                        if (cmb.Template.FindName("PART_Border", cmb) is Border border)
                        {
                            border.BorderBrush = (IsReadOnly is true && !cmb.Items.Contains(inputText))?
                                Brushes.Red : Brushes.Black;
                        }
                        cmb.BorderBrush = (IsReadOnly is true && !cmb.Items.Contains(inputText))?
                            Brushes.Red : Brushes.Black;

                        UpdateOptions?.Invoke(inputText);
                        textbox.Text = inputText;
                        if (textbox.SelectionLength < 1)
                            SetCaret(textbox, caretIndex);
                        break;
                }
            }
        }

        private void DropdownInput_Paste(object sender, DataObjectPastingEventArgs e)
        {
            if(sender is ComboBox cmb)
            {
                TextBox? textbox = cmb.Template.FindName("PART_EditableTextBox", cmb) as TextBox;
                if (textbox == null) return;
            
                string pastedText = (string)e.DataObject.GetData(typeof(string));
                e.CancelCommand();

                int selectionStart = textbox.SelectionStart;
                int selectionLength = textbox.SelectionLength;
                string text = textbox.Text.Substring(0, selectionStart) +
                    pastedText +
                    textbox.Text.Substring(selectionStart + selectionLength);
                textbox.Text = text;
                inputText = text;

                SetCaret(textbox, selectionStart + pastedText.Length);
                cmb.IsDropDownOpen = cmb.Text?.Length > 1;
            }
        }

        private void DropdownInputItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ComboBoxItem item && item.DataContext is string option)
            {
                TextBox? textbox = TitleInput.Template.FindName("PART_EditableTextBox", TitleInput) as TextBox;
                if (textbox == null) return;

                textbox.Text = option;
                SetCaret(textbox);
                OnOptionSelected?.Invoke(option,inputText);
                inputText = option;
            }
        }

        private void DropdownInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is ComboBox cmb)
            {
                TextBox? textbox = cmb.Template.FindName("PART_EditableTextBox", cmb) as TextBox;
                if (textbox == null) return;
                SetCaret(textbox);
            }
        }

        private void DropdownInput_DropDownOpened(object sender, EventArgs e)
        {
            if (sender is ComboBox cmb)
            {
                TextBox? textbox = cmb.Template.FindName("PART_EditableTextBox", cmb) as TextBox;
                if (textbox == null) return;
                SetCaret(textbox, caretIndex);
            }
        }

        private void SetCaret(TextBox textbox, int position = -1)
        {
            position = (position < 0 || position > textbox.Text.Length) ? textbox.Text.Length : position;
            textbox.SelectionStart = 0;
            textbox.CaretIndex = position;
        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {
            if(sender is ComboBox cmb && IsEditable)
            {
                cmb.IsDropDownOpen = true;
            }
            e.Handled = true;
        }

        // not required
        private void LostFocus(object sender, RoutedEventArgs e)
        {
            if(sender is ComboBox cmb)
            {
                cmb.Text = Text;
                cmb.IsDropDownOpen = false;
            }
            e.Handled = true;
        }
    }
}
