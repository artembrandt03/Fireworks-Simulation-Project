using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("FireworksTest")]

namespace Fireworks
{
    //FireworkEnvironment: manages all active fireworks in the simulation
    public class FireworkEnvironment
    {
        //---FIELDS and PROPERTIES------------------------------------
        private readonly List<IFirework> _fireworks;
        public List<IFirework> Items { get { return _fireworks; } }
        //-------------------------------------------------------------

        //---CONSTRUCTOR----------------------------------------------
        public FireworkEnvironment()
        {
            _fireworks = new List<IFirework>();
        }
        //-------------------------------------------------------------

        //---METHODS--------------------------------------------------
        //Launches the firework and adds it to the list
        public void AddFirework(IFirework f)
        {
            if (f == null)
            {
                return;
            }

            f.Launch();
            _fireworks.Add(f);
        }

        //Updates the state of fireworks
        public void Update()
        {
            int count = _fireworks.Count;
            if (count == 0)
            {
                return;
            }

            //1) Update each firework in parallel.
            //Fireworks are independent, so this is safe
            Parallel.For(0, count, i =>
            {
                _fireworks[i].Update();
            });

            // 2) Remove finished fireworks (exploded and no particles left)
            //This part is sequential so we can safely modify the list.
            for (int i = _fireworks.Count - 1; i >= 0; i--)
            {
                IFirework fw = _fireworks[i];
                if (fw.Exploded && fw.Particles.Count == 0)
                {
                    _fireworks.RemoveAt(i);
                }
            }
        }

        //Prevents memory overload when too many fireworks exist (50)
        public void Clear()
        {
            if (_fireworks.Count > 50)
            {
                _fireworks.Clear();
            }
        }
        //-------------------------------------------------------------
    }
}
