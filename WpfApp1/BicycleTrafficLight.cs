using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class BicycleTrafficLight : TrafficLight
    {
        private Vector2 loopfrontpos;
        private bool loopFront;

        public BicycleTrafficLight(int id, char groep, Vector2 loopfrontpos) : base(id, groep, Direction.straight, typeTrafficlight.Cyclistlight)
        {
            this.loopfrontpos = loopfrontpos;

            loopFront = false;

        }

        public override string fullId()
        {
            return base.fullId()+"bycle";
        }
        //todo loops

        public override Info GetRoadInfo()
        {
            return new CyclistRoadInfo(loopFront);

        }
    }
}
