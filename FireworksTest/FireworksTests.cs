using Fireworks;
using ShapeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fireworks;
using ShapeLibrary;

namespace FireworksTest
{
    [TestClass]
    public class FireworkTests
    {
        [TestMethod]
        public void Launch_CreatesLauncher_WithUpwardVelocity()
        {
            var colour = new Colour(200, 100, 50);
            var pattern = new ExplosionPattern();
            var fw = new Firework(800, 600, colour, pattern);

            fw.Launch();

            Assert.IsNotNull(fw.Launcher);
            Assert.IsTrue(fw.Launcher.Velocity.y < 0f);
            Assert.IsFalse(fw.Exploded);
        }

        [TestMethod]
        public void Update_EventuallyExplodes_AndCreatesParticles()
        {
            var colour = new Colour(150, 150, 255);
            var pattern = new ExplosionPattern();

            var fw = new Firework(800, 600, colour, pattern);
            fw.Launch();

            bool exploded = false;
            for (int i = 0; i < 300; i++)
            {
                fw.Update();
                if (fw.Exploded)
                {
                    exploded = true;
                    break;
                }
            }

            Assert.IsTrue(exploded);
            Assert.IsTrue(fw.Particles.Count >= 60);
            Assert.IsTrue(fw.Particles.Count <= 120);
            Assert.IsNull(fw.Launcher);
        }

        [TestMethod]
        public void AfterExplosion_ParticlesEventuallyExpire()
        {
            var colour = new Colour(255, 200, 50);
            var pattern = new ExplosionPattern();
            var fw = new Firework(800, 600, colour, pattern);
            fw.Launch();

            // advance until explosion
            for (int i = 0; i < 400 && !fw.Exploded; i++)
            {
                fw.Update();
            }
            Assert.IsTrue(fw.Exploded);

            //testing for Firework.Update
            for (int i = 0; i < 400; i++)
            {
                fw.Update();
            }
            Assert.AreEqual(0, fw.Particles.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void Constructor_Throws_ForInvalidDimensions()
        {
            var colour = new Colour(0, 0, 0);
            var pattern = new ExplosionPattern();
            var fw = new Firework(0, 0, colour, pattern);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void Constructor_Throws_ForNullPattern()
        {
            var fw = new Firework(800, 600, new Colour(0, 0, 0), null);
        }
    }
}
