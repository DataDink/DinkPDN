using PaintDotNet;
using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DinkPDN.Effects.Simple
{
    public class ConfigurableVectorAttribute : ConfigurableAttribute
    {
        public double X { get; set; }
        public double Y { get; set; }
        public object MaxX { get; set; }
        public object MaxY { get; set; }
        public object MinX { get; set; }
        public object MinY { get; set; }

        public override PropertyControl Create(PropertyInfo property)
        {
            var info = Configure(new DoubleVectorProperty(
                property.Name,
                new Pair<double, double>(X, Y),
                new Pair<double, double>(MinX.GetValue(double.MinValue), MinY.GetValue(double.MinValue)),
                new Pair<double, double>(MaxX.GetValue(double.MaxValue), MaxY.GetValue(double.MaxValue))));
            return (PropertyControl)info.CreateConcreteControl(UISystem);
        }
    }

    public class ConfigurableVector3Attribute : ConfigurableVectorAttribute
    {
        public double Z { get; set; }
        public object MaxZ { get; set; }
        public object MinZ { get; set; }

        public override PropertyControl Create(PropertyInfo property)
        {
            var info = Configure(new DoubleVector3Property(
                property.Name,
                new Tuple<double, double, double>(X, Y, Z),
                new Tuple<double, double, double>(MinX.GetValue(double.MinValue), MinY.GetValue(double.MinValue), MinZ.GetValue(double.MinValue)),
                new Tuple<double, double, double>(MaxX.GetValue(double.MaxValue), MaxY.GetValue(double.MaxValue), MaxZ.GetValue(double.MaxValue))));
            return (PropertyControl)info.CreateConcreteControl(UISystem);
        }
    }

}
