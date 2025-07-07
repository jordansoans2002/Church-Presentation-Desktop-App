using create_ppt_app.Model;
using create_ppt_app.MVVM;
using System.Diagnostics;
using System.Windows.Input;

namespace create_ppt_app.ViewModel
{
    public class SettingViewModel : ViewModelBase
    {
        public Setting Setting { get; private set; }
        public string Type 
        {
            get { return Setting.SettingType.ToString(); }
        }
        public string SettingName
        { 
            get { return Setting.SettingName; } 
        }
        public string DisplayValue
        {
            get {
                if (Options != null)
                {
                    var item = Options.FirstOrDefault(kvp => kvp.Value == SettingValue);
                    if (!item.Equals(default(KeyValuePair<string, string>)))
                    {
                        return item.Key;
                    }
                }
                return Setting.SettingValue; // string.Empty;
            }
            set
            {
                SettingValue = value;
                OnPropertyChanged();
            }

        }

        public string SettingValue
        {
            get{ return Setting.SettingValue; }
            set
            {
                if (Options != null)
                {
                    var item = Options.FirstOrDefault(kvp => kvp.Key == value);
                    if (!item.Equals(default(KeyValuePair<string, string>)))
                    {
                        Setting.SettingValue = item.Value;
                    }
                }
                else
                {
                    Setting.SettingValue = value;
                }
                OnPropertyChanged();
            }
        }
        public ICommand PickerCommand { get; }
        public List<KeyValuePair<string, string>>? Options
        {
            get { return Setting.Options; }
            set
            {
                Setting.Options = value;
                OnPropertyChanged();
            }
        }
        public List<string> DropdownOptions { 
            get{
                if (Options != null)
                {
                    return Options.Select(dict => dict.Key).ToList();
                }
                else
                    return new List<string> { };
            } 
        }
        public Func<string, string?>? Validation { get; set; }
       
        public SettingViewModel(SettingViewModel model)
        {
            this.Setting = new Setting(model.Setting);
            if (model.Setting.SettingType == SettingType.Picker && model.PickerCommand is ICommand command)
                this.PickerCommand = model.PickerCommand;
            if (model.Setting.SettingType == SettingType.TextInput && model.Validation is Func<string, string?> v)
                this.Validation = model.Validation;
        }

        public SettingViewModel(Setting setting, object? o=null)
        {
            this.Setting = setting;
            if (setting.SettingType == SettingType.Picker && o is ICommand command)
                PickerCommand = command;
            if (setting.SettingType == SettingType.TextInput && o is Func<string, string?> v)
                Validation = v;
        }

    }
}
