using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Markup;

namespace VRCToolBox.Pictures.Shared
{
    // reference : https://lifetime-engineer.com/wpf-enum-combobox/
    // reference : https://qiita.com/flasksrw/items/f3bd8153c32dbcdfc7fb
    internal static class EnumExtensions
    {
        internal static string GetName<T>(this T Value) where T : struct
        {
            if (!(typeof(T).IsEnum)) throw new InvalidOperationException();

            var fieldInfo = Value.GetType().GetField(Value.ToString() ?? string.Empty);
            if (fieldInfo == null) return string.Empty;

            var attr = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
            return attr?.Description ?? string.Empty;
        }
    }
}
