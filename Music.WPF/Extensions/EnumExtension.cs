using Music.WPF.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Music.WPF.Extensions
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string? name = Enum.GetName(type, value);
            if (name is not null)
            {
                FieldInfo? field = type.GetField(name);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                    {
                        return attr.Description;
                    }
                }
            }
            return string.Empty;
        }

        public static List<string> GetSortOptions()
        {
            var values = Enum.GetValues(typeof(SortOption));

            List<string> options = new();

            foreach (var item in values)
            {
                options.Add(GetDescription((SortOption)item));
            }

            return options;
        }
        public static T? GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T?)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T?)field.GetValue(null);
                }
            }

            return default;
        }

    }
}
