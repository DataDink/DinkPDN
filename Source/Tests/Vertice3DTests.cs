using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DinkPDN.Plotting.X3D;

namespace Tests
{
    [TestClass]
    public class Vertice3DTests
    {
        [TestMethod]
        public void Axis3D()
        {
            Assert.IsTrue(new Vertice(0, -1, 0).ZAxis == 0);
            Assert.IsTrue(new Vertice(1, -1, 0).ZAxis == 45);
            Assert.IsTrue(new Vertice(1, 0, 0).ZAxis == 90);
            Assert.IsTrue(new Vertice(1, 1, 0).ZAxis == 135);
            Assert.IsTrue(new Vertice(0, 1, 0).ZAxis == 180);
            Assert.IsTrue(new Vertice(-1, 1, 0).ZAxis == 225);
            Assert.IsTrue(new Vertice(-1, 0, 0).ZAxis == 270);
            Assert.IsTrue(new Vertice(-1, -1, 0).ZAxis == 315);

            Assert.IsTrue(new Vertice(0, -1, 0).XAxis == 0);
            Assert.IsTrue(new Vertice(0, -1, 1).XAxis == 45);
            Assert.IsTrue(new Vertice(0, 0, 1).XAxis == 90);
            Assert.IsTrue(new Vertice(0, 1, 1).XAxis == 135);
            Assert.IsTrue(new Vertice(0, 1, 0).XAxis == 180);
            Assert.IsTrue(new Vertice(0, 1, -1).XAxis == 225);
            Assert.IsTrue(new Vertice(0, 0, -1).XAxis == 270);
            Assert.IsTrue(new Vertice(0, -1, -1).XAxis == 315);

            Assert.IsTrue(new Vertice(0, 0, -1).YAxis == 0);
            Assert.IsTrue(new Vertice(1, 0, -1).YAxis == 45);
            Assert.IsTrue(new Vertice(1, 0, 0).YAxis == 90);
            Assert.IsTrue(new Vertice(1, 0, 1).YAxis == 135);
            Assert.IsTrue(new Vertice(0, 0, 1).YAxis == 180);
            Assert.IsTrue(new Vertice(-1, 0, 1).YAxis == 225);
            Assert.IsTrue(new Vertice(-1, 0, 0).YAxis == 270);
            Assert.IsTrue(new Vertice(-1, 0, -1).YAxis == 315);
        }
    }
}
