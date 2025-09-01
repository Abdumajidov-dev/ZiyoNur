using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Entities.System
{
    public class SystemSetting : BaseAuditableEntity
    {
        [Required]
        [MaxLength(100)]
        public string SettingKey { get; set; } = string.Empty;

        [Required]
        public string SettingValue { get; set; } = string.Empty;

        public string? Description { get; set; }

        [MaxLength(20)]
        public string DataType { get; set; } = "string"; // string, integer, decimal, boolean

        public bool IsPublic { get; set; } = false; // can be accessed by mobile apps

        // Business Methods
        public T GetValue<T>()
        {
            return DataType switch
            {
                "integer" => (T)(object)int.Parse(SettingValue),
                "decimal" => (T)(object)decimal.Parse(SettingValue),
                "boolean" => (T)(object)bool.Parse(SettingValue),
                _ => (T)(object)SettingValue
            };
        }

        public void SetValue<T>(T value)
        {
            SettingValue = value?.ToString() ?? string.Empty;
            DataType = typeof(T).Name.ToLower();
        }
    }
}
