using create_ppt_app.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace create_ppt_app.Model
{
    public class Setting
    {
        public string SettingName { get; set; }
        public string SettingValue { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SettingType SettingType { get; set; }
        public List<KeyValuePair<string, string>>? Options { get; set; }

        public Setting() { }
        public Setting(Setting model)
        {
            this.SettingName = model.SettingName;
            this.SettingValue = model.SettingValue;
            this.SettingType = model.SettingType;
            if (model.SettingType == SettingType.Dropdown || model.SettingType == SettingType.TextInputDropdown)
            {
                this.Options = model.Options;
            }
        }
        public Setting(string settingName, string defaultValue, SettingType type, List<KeyValuePair<string, string>> options = null)
        {
            this.SettingName = settingName;
            this.SettingValue = defaultValue;
            this.SettingType = type;
            if (type == SettingType.Dropdown || type == SettingType.TextInputDropdown)
            {
                this.Options = options;
            }
        }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SettingType
    {
        Dropdown,
        TextInputDropdown,
        Picker,
        ColorPicker,
        Checkbox,
        TextInput,
        Range
    }
    public enum LyricsSource
    {
        GoogleDrive,
        LocalFolder
    }

    public enum SlideRatio
    {
        _4x3,
        _16x9
    }
    public static class SlideRatioExtension
    {
        public static (int Width, int Height) GetDimensions(this SlideRatio slideRatio)
        {
            return slideRatio switch
            {
                SlideRatio._4x3 => (4, 3),
                SlideRatio._16x9 => (960, 540),
                _ => throw new NotImplementedException()
            };
        }
    }
}
