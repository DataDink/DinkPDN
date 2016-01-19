using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DinkPDN.UI.Controls
{
    public class PropertyControlDescriptor
    {
        private PropertyControlDescriptor() { }
        public int UserExperience { get; private set; }
        public Type ControlType { get; private set; }
        public Type PropertyType { get; private set; }
        public Type ValueType { get; private set; }

        private static readonly string SearchPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private static readonly Assembly[] Assemblies = Directory.GetFiles(SearchPath, "*", SearchOption.AllDirectories)
                .Select(f => { try { return Assembly.LoadFile(f); } catch { return null; } })
                .Where(a => a != null).ToArray();
        private static readonly Type[] InternalTypes = Assemblies.SelectMany(p => p.GetTypes())
            .Where(t => typeof(PropertyControl).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
            .ToArray();

        private static PropertyControlDescriptor[] _cached;
        public static PropertyControlDescriptor[] All
        {
            get {
                return _cached ?? (_cached = InternalTypes
                .Select(t => new { type = t, attr = t.GetCustomAttribute<PropertyControlInfoAttribute>() })
                .Where(t => t.attr != null)
                .Select(t => new PropertyControlDescriptor {
                    UserExperience = (int)t.attr.ControlType,
                    ControlType = t.type,
                    PropertyType = t.attr.PropertyType,
                    ValueType = GetPropertyType(t.type)
                }).ToArray());
            }
        }

        private static Type GetPropertyType(Type type)
        {
            while (type != null && type.BaseType != typeof(PropertyControl)) type = type.BaseType;
            if (type == null) return null;
            var desc = type.GetGenericArguments().FirstOrDefault();
            return desc;
        }

        public enum ExperienceTypes : int
        {
            AngleChooser = 0,
            CheckBox = 1,
            PanAndSlider = 2,
            Slider = 3,
            IncrementButton = 4,
            DropDown = 5,
            TextBox = 6,
            RadioButton = 7,
            ColorWheel = 8,
            RollBallAndSliders = 9,
            /****************************************************************************/

            FileBrowser = 65536
        }
    }
}
