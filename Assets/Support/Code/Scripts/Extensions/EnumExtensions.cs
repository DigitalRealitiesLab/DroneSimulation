using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;

namespace Support.Extensions {
    public static class EnumExtensions {
        public static IEnumerable<Enum> ForEach(this Enum @enum) => Enum.GetValues(@enum.GetType()).Cast<Enum>().Where(@enum.HasFlag);

        public static string AsString(this Enum @enum) {
            Type type = @enum.GetType();
            string name = Enum.GetName(type, @enum);
            if (name == null) {
                return string.Empty;
            }

            FieldInfo field = type.GetField(name);
            if (field == null) {
                return string.Empty;
            }

            return Attribute.GetCustomAttribute(field, typeof(StringRepresentation)) is StringRepresentation attr ? attr.Representation : string.Empty;
        }

        public static void PopulateDropdown(this Enum @enum, TMP_Dropdown dropdown) {
            Type enumType = @enum.GetType();
            var newOptions = new List<TMP_Dropdown.OptionData>();

            for (var i = 0; i < Enum.GetNames(enumType).Length; i++) {
                newOptions.Add(new TMP_Dropdown.OptionData(Enum.GetName(enumType, i)));
            }

            dropdown.ClearOptions();
            dropdown.AddOptions(newOptions);
        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
        public class StringRepresentation : Attribute {
            public StringRepresentation(string representation) => Representation = representation;

            internal string Representation { get; }
        }
    }
}