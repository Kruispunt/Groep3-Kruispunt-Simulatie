using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class PedestrianTrafficlight : TrafficLight
    {

        private Vector2 loopfrontpos;
        private bool loopFront;
        private Pedestrian Onfrontloop;


        public PedestrianTrafficlight(int id, char groep, Vector2 loopfrontpos) : base(id, groep, Direction.straight, typeTrafficlight.PedestrianLight)
        {
            this.loopfrontpos = loopfrontpos;

            loopFront = false;
        }

        public override string fullId()
        {
            return base.fullId()+ "Pedes";
        }

        //todo loops

        public override Info GetRoadInfo()
        {
            return new pedesRoadInfo(loopFront);

        }
        public override void SetLoop(List<Vehicle> vehicles)
        {
            List<Pedestrian> bikes = vehicles.OfType<Pedestrian>().ToList();

            foreach (Pedestrian pedes in bikes)
            {
                if (pedes.onpoint(loopfrontpos) && loopFront == false)
                {
                    loopFront = true;
                    pedes.setwaitingtrafficlight(this);
                    Onfrontloop = pedes;

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
