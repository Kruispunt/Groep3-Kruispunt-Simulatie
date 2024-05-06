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
        private Bycicle Onfrontloop;

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
        public override void SetLoop(List<Vehicle> vehicles)
        {
            List<Bycicle> bikes = vehicles.OfType<Bycicle>().ToList();

            foreach (Bycicle bike in bikes)
            {
                if (bike.onpoint(loopfrontpos) && loopFront == false)
                {
                    loopFront = true;
                    bike.setwaitingtrafficlight(this);
                    Onfrontloop = bike;
                    //if (getDirection() == Direction.right || getDirection() == Direction.left)
                    //{
                        //bike.setturningpoint(turnposition, futuredirection);
                    //}

                }
                else if (Onfrontloop != null && !Onfrontloop.onpoint(loopfrontpos))
                {
                    loopFront = false;
                    Onfrontloop = null;
                }

            }
        }
    }
}
