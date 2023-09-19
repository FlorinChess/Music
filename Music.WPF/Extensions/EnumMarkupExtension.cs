using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;

namespace Music.WPF.Extensions
{
    public sealed class EnumMarkupExtension : MarkupExtension
    {
        public Type EnumType { get; private set; }

        public EnumMarkupExtension(Type enumType)
        {
            if (enumType is null || !enumType.IsEnum)
                throw new Exception("Type is null or is not an Enum!");

            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType).Cast<Enum>().Select(EnumToDescriptionOrString);
        }
        //.Cast<Enum>().Select(EnumToDescriptionOrString);
        private string EnumToDescriptionOrString(Enum value)
        {
            return value.GetType().GetField(value.ToString())
                       .GetCustomAttributes(typeof(DescriptionAttribute), false)
                       .Cast<DescriptionAttribute>()
                       .FirstOrDefault()?.Description ?? value.ToString();
        }
    }
}
