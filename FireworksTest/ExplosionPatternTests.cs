using Fireworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShapeLibrary;

namespace FireworksTest
{
    [TestClass]
    public class ExplosionPatternTests
    {
        [TestMethod]
        public void LaunchVelocity_IsUpwardOnlyOnY()
        {
            var p = new ExplosionPattern();

            Assert.AreEqual(0f, p.LaunchVelocity.x);
            Assert.IsTrue(p.LaunchVelocity.y < 0f);
        }

        [TestMethod]
        public void NumberOfParticles_IsWithinExpectedRange()
        {
            var p = new ExplosionPattern();

            Assert.IsTrue(p.NumberOfParticles >= 60);
            Assert.IsTrue(p.NumberOfParticles <= 100);
        }

        [TestMethod]
        public void ExplosionVelocity_ProducesDirectionsAboveAndBelow()
        {
            var p = new ExplosionPattern();

            bool sawPositiveY = false;
            bool sawNegativeY = false;

            for (int i = 0; i < 200; i++)
            {
                Vector v = p.ExplosionVelocity;
                if (v.y > 0f)
                {
                    sawPositiveY = true;
                }
                if (v.y < 0f)
                {
                    sawNegativeY = true;
                }
            }

            Assert.IsTrue(sawPositiveY);
            Assert.IsTrue(sawNegativeY);
        }
    }
}
