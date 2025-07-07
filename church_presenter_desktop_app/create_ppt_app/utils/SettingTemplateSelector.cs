
using System.Windows.Controls;
using System.Windows;
using create_ppt_app.Model;
using create_ppt_app.ViewModel;

namespace create_ppt_app.utils
{
    public class SettingTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextTemplate { get; set; }
        public DataTemplate DropdownTemplate { get; set; }
        public DataTemplate TextInputDropdownTemplate { get; set; }
        public DataTemplate ColorPickerTemplate { get; set; }
        public DataTemplate PickerTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is SettingViewModel setting)
            {
                return Enum.Parse(typeof(SettingType),setting.Type) switch
                {
                    SettingType.TextInput => TextTemplate,
                    SettingType.Dropdown => DropdownTemplate,
                    SettingType.TextInputDropdown => TextInputDropdownTemplate,
                    SettingType.ColorPicker => ColorPickerTemplate,
                    SettingType.Picker => PickerTemplate,
                    _ => TextTemplate
                };
            }
            return base.SelectTemplate(item, container);
        }
    }
}
