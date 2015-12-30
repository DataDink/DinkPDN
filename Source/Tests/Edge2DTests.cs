using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DinkPDN.Plotting.X2D;

namespace Tests
{
    [TestClass]
    public class Edge2DTests
    {
        [TestMethod]
        public void Intersect2D()
        {
            var a = new Edge(new Vertice(-1, -1), new Vertice(1, 1));
            var b = new Edge(new Vertice(-1, 1), new Vertice(1, -1));
            var x = a.Intersect(b);
            Assert.IsTrue(x.HasValue && x.Value.X == 0 && x.Value.Y == 0);

            a = new Edge(new Vertice(-1, -1), new Vertice(-1, 1));
            b = new Edge(new Vertice(1, -1), new Vertice(1, 1));
            x = a.Intersect(b);
            Assert.IsFalse(x.HasValue);
        }
    }
}
