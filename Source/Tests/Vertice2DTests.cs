using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DinkPDN.Plotting.X2D;

namespace Tests
{
    [TestClass]
    public class Vertice2DTests
    {
        [TestMethod]
        public void Axis2D()
        {
            Assert.IsTrue(new Vertice(0, -1).ZAxis == 0);
            Assert.IsTrue(new Vertice(1, -1).ZAxis == 45);
            Assert.IsTrue(new Vertice(1, 0).ZAxis == 90);
            Assert.IsTrue(new Vertice(1, 1).ZAxis == 135);
            Assert.IsTrue(new Vertice(0, 1).ZAxis == 180);
            Assert.IsTrue(new Vertice(-1, 1).ZAxis == 225);
            Assert.IsTrue(new Vertice(-1, 0).ZAxis == 270);
            Assert.IsTrue(new Vertice(-1, -1).ZAxis == 315);
        }

        [TestMethod]
        public void Rotate2D()
        {
            var x = 0d; var y = -1d;
            VerticeExtensions.Rotate(0, x, y, out x, out y);
            Assert.IsTrue(x == 0 && y == -1);
            VerticeExtensions.Rotate(45, x, y, out x, out y);
            Assert.IsTrue(x > 0 && y < 0 && Math.Round(x + y, 15) == 0);
            VerticeExtensions.Rotate(45, x, y, out x, out y);
            x = Math.Round(x, 10); y = Math.Round(y, 10);
            Assert.IsTrue(x == 1 && y == 0);
            VerticeExtensions.Rotate(180, x, y, out x, out y);
            x = Math.Round(x, 10); y = Math.Round(y, 10);
            Assert.IsTrue(x == -1 && y == 0);
        }

        [TestMethod]
        public void Calculate2D()
        {
            foreach (var test in new[] {
                new Vertice(1, 1),
                new Vertice(0, 1),
                new Vertice(1, 0),
                new Vertice(-1, 0),
                new Vertice(0, -1),
                new Vertice(-1, -1),
                new Vertice(1, -1),
                new Vertice(-1, 1),
                new Vertice(50, 5),
                new Vertice(-50, 5),
                new Vertice(50, -5),
                new Vertice(-50, -5),
                new Vertice(5, 50),
                new Vertice(-5, 50),
                new Vertice(5, -50),
                new Vertice(-5, -50),
            }) {
                var result = VerticeExtensions.Calculate(test.ZAxis, test.Length);
                Assert.IsTrue(
                    Math.Round(result.Length - test.Length, 5) == 0
                    && Math.Round(result.ZAxis - test.ZAxis, 5) == 0
                    && Math.Round(result.X - test.X, 5) == 0
                    && Math.Round(result.Y - test.Y, 5) == 0);
            }
        }
    }
}
