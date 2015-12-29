using PaintDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DinkPDN.Common
{
    public interface IPluginConfiguration : IPluginSupportInfo
    {
        Image Icon { get; }
    }

    public class AssemblyPluginSupportInfo : IPluginConfiguration
    {
        private static readonly Assembly Info = Assembly.GetExecutingAssembly();

        public virtual Version Version { get { return Info.GetName().Version; } }
        public virtual string Author { get { return Get<AssemblyCompanyAttribute>(a => a.Company) ?? "DataDink"; } }
        public virtual string Copyright { get { return Get<AssemblyCopyrightAttribute>(a => a.Copyright) ?? "2015 / MIT"; } }
        public virtual string DisplayName { get { return Get<AssemblyTitleAttribute>(a => a.Title) ?? "Unknown"; } }
        public virtual Uri WebsiteUri { get { return new Uri("https://github.com/DataDink/DinkPDN"); } }
        public virtual Image Icon {
            get {
                var resources = Info.GetManifestResourceNames();
                var name = resources.FirstOrDefault(n => n.EndsWith(".icon.png"));
                if (name == null) return new Bitmap(16, 16);
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name)) {
                    return Image.FromStream(stream);
                }
            }
        }

        private static string Get<TAttribute>(Func<TAttribute, string> getter) where TAttribute : Attribute
        {
            var attribute = Info.GetCustomAttribute<TAttribute>();
            return attribute != null ? getter(attribute) : null;
        }

    }
}
