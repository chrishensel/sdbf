using System;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SimpleDosboxFrontend
{
    static class Extensions
    {
        public static T TryGetElementValue<T>(this XElement root, string name, T defaultValue)
        {
            var element = root.Element(name);

            if (element != null)
            {
                return (T)Convert.ChangeType(element.Value, typeof(T));
            }

            return defaultValue;
        }

        public static T TryGetAttributeValue<T>(this XElement root, string name, T defaultValue)
        {
            var element = root.Attribute(name);

            if (element != null)
            {
                return (T)Convert.ChangeType(element.Value, typeof(T));
            }

            return defaultValue;
        }

        public static T FindControl<T>(this Control control, string name) where T : Control
        {
            return FindControl(control, name) as T;
        }

        private static Control FindControl(Control source, string name)
        {
            foreach (Control child in source.Controls)
            {
                var ctrl = FindControl(child, name);

                if (ctrl != null)
                {
                    return ctrl;
                }
            }

            if (source.Name == name)
            {
                return source;
            }

            return null;
        }
    }
}