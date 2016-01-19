using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DinkPDN.UI.Controls
{
    public abstract class PropertyControlHost : Panel
    {
        private readonly PropertyControlDescriptor _descriptor;
        private readonly PropertyControlInfo _info;
        private readonly PropertyControl _control;

        public string PropertyName { get { return _info.Property.Name; } }
        public PropertyControlInfo Info { get; private set; }

        protected PropertyControlHost(PropertyControlDescriptor descriptor, Property property)
        {
            _descriptor = descriptor;
            _info = PropertyControlInfo.CreateFor(property);
            _control = (PropertyControl)Activator.CreateInstance(_descriptor.ControlType, _info);
            
        }
    }
}
