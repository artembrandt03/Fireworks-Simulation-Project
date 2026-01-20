using Fireworks;
using ShapeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireworksTest
{
    [TestClass]
    public class FireworkEnvironmentTests
    {
        [TestMethod]
        public void AddFirework_Launches_AndAdds()
        {
            var env = new FireworkEnvironment();
            var fw = new Firework(800, 600, new Colour(120, 150, 200), new ExplosionPattern());

            env.AddFirework(fw);

            Assert.AreEqual(1, env.Items.Count);
            Assert.IsNotNull(env.Items[0].Launcher);
        }

        [TestMethod]
        public void Update_RemovesFinishedFireworks()
        {
            var env = new FireworkEnvironment();
            var fw = new Firework(800, 600, new Colour(200, 200, 200), new ExplosionPattern());
            env.AddFirework(fw);

            //run enough frames for launch, explosion, and particle expiry
            for (int i = 0; i < 800; i++)
            {
                env.Update();
            }

            //one more to test update to trigger cleanup when empty
            env.Update();

            Assert.AreEqual(0, env.Items.Count);
        }

        [TestMethod]
        public void Clear_TrimsWhenTooMany()
        {
            var env = new FireworkEnvironment();

            //add 55 fireworks
            for (int i = 0; i < 55; i++)
            {
                env.AddFirework(new Firework(800, 600, new Colour(100, 100, 100), new ExplosionPattern()));
            }

            env.Clear();
            //should trim to 50 instead
            Assert.IsTrue(env.Items.Count <= 50);
        }
    }
}
