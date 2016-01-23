using DinkPDN.Common;
using PaintDotNet;
using PaintDotNet.Effects;
using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DinkPDN.Effects.Simple
{
    [PluginSupportInfo(typeof(AssemblyPluginSupportInfo))]
    public abstract class ConfigurableEffect : ConfigurableEffect<AssemblyPluginSupportInfo>
    {
        protected ConfigurableEffect() : base() { }
        protected ConfigurableEffect(string menu) : base(menu) { }
        protected ConfigurableEffect(string name, string menu) : base(name, menu) { }
        protected ConfigurableEffect(string name, Image image, string menu) : base(name, image, menu) { }
    }

    public abstract class ConfigurableEffect<TPluginInfo> : BaseConfigurableEffect
        where TPluginInfo : IPluginConfiguration, new()
    {
        private static readonly TPluginInfo PluginInfo = new TPluginInfo();
        protected ConfigurableEffect() : this(null) { }
        protected ConfigurableEffect(string menu) : this(PluginInfo.DisplayName, menu) { }
        protected ConfigurableEffect(string name, string menu) : this(name, PluginInfo.Icon, menu) { }
        protected ConfigurableEffect(string name, Image image, string menu) : base(name, image, menu) { }
    }

    public abstract class BaseConfigurableEffect : Effect, INotifyPropertyChanged
    {
        private readonly Dictionary<PropertyInfo, ConfigurableAttribute> Properties;
        private Dialog EffectDialog { get; set; }

        protected BaseConfigurableEffect(string name, Image image, string menu) : base(name, image, menu, EffectFlags.Configurable) {
            Properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new { prop = p, attr = p.GetCustomAttribute<ConfigurableAttribute>(true) })
                .Where(p => p.attr != null && p.prop.CanWrite)
                .ToDictionary(p => p.prop, p => p.attr);
        }

        protected virtual void OnPropertyChanged(PropertyInfo property, object oldValue, object newValue)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property.Name));
            EffectDialog.TriggerUpdate();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override EffectConfigDialog CreateConfigDialog()
        {
            EffectDialog = new Dialog() { Effect = this, Text = this.Name };
            foreach (var kvp in Properties) {
                var prop = kvp.Key;
                var attr = kvp.Value;
                var control = attr.Create(prop);
                control.Property.ValueChanged += (s, e) => {
                    var oldvalue = prop.GetValue(this, null);
                    prop.SetValue(this, control.Property.Value);
                    OnPropertyChanged(prop, oldvalue, control.Property.Value);
                };
                var defaulted = attr as DefaultConfigurableAttribute;
                if (defaulted != null && defaulted.Default != null) {
                    prop.SetValue(this, defaulted.Default);
                } else if (control.Property.Value != null) {
                    prop.SetValue(this, control.Property.Value);
                }
                control.Dock = DockStyle.Top;
                EffectDialog.Controls.Add(control);
            }
            return EffectDialog;
        }

        protected override void OnSetRenderInfo(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            lock(_readylock)
            if (!_ready) {
                _ready = true;
                // Any future prep should go / be called here
                OnReady(srcArgs, dstArgs);
            }
            base.OnSetRenderInfo(parameters, dstArgs, srcArgs);
        }

        private bool _ready;
        private readonly object _readylock = new object();
        protected virtual void OnReady(RenderArgs src, RenderArgs dst) { }

        public sealed override void Render(EffectConfigToken parameters, RenderArgs dstArgs, RenderArgs srcArgs, Rectangle[] rois, int startIndex, int length)
        {
            Render(rois.Skip(startIndex).Take(length).ToArray(), dstArgs, srcArgs);
        }

        protected abstract void Render(Rectangle[] rects, RenderArgs dst, RenderArgs src);

        protected override void OnDispose(bool disposing)
        {
            if (EffectDialog != null) EffectDialog.Dispose();
            base.OnDispose(disposing);
        }

        private class Dialog : EffectConfigDialog {
            private readonly Panel Footer = new Panel { Dock = DockStyle.Top, Height = 40 };
            private readonly Button OK = new Button { Text = "OK", Dock = DockStyle.Right };
            private readonly Button Cancel = new Button { Text = "Cancel", Dock = DockStyle.Right };

            public Dialog()
            {
                EffectToken = new Token();
                Padding = new Padding(5, 10, 5, 10);
                Size = new Size(450, 0);
                MinimumSize = Size;
                AutoSize = true;
                AutoSizeMode = AutoSizeMode.GrowAndShrink;
                BackColor = Color.White;

                Controls.Add(Footer);
                Footer.Controls.Add(OK);
                Footer.Controls.Add(new Label { BorderStyle = BorderStyle.None, Width = 10, Dock = DockStyle.Right });
                Footer.Controls.Add(Cancel);
                Footer.Controls.Add(new Label { BorderStyle = BorderStyle.None, Height = 10, Dock = DockStyle.Top });
                Footer.Controls.Add(new Label { BorderStyle = BorderStyle.Fixed3D, Height = 2, Dock = DockStyle.Top });

                Cancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
                OK.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };
            }

            public void TriggerUpdate()
            {
                OnEffectTokenChanged();
            }

            private bool _suspend;
            protected override void OnControlAdded(ControlEventArgs e)
            {

                base.OnControlAdded(e);
                if (_suspend || e.Control == Footer) return;
                _suspend = true;
                e.Control.Dock = DockStyle.Top;
                e.Control.BringToFront();

                var margin = new Panel { Dock = DockStyle.Top, Height = 15 };
                Controls.Add(margin);
                margin.BringToFront();

                Footer.BringToFront();
                _suspend = false;
            }

            private class Token : EffectConfigToken { public override object Clone() { return new Token(); } }
        }
    }
}
