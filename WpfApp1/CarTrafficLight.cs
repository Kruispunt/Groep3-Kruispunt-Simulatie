using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Numerics;

namespace WpfApp1
{
    public class CarTrafficLight: TrafficLight
    {

        private bool loopFront;
        private bool loopBack;

        private Car Onfrontloop;
        private Car Onbackloop;

        private Vector2 turnposition;
        private Vector2 loopfrontpos;//first loop point
        private Vector2 loopbackpos;//second loop point


        private Drivedirection futuredirection;

        public CarTrafficLight(int id,char groep, Vector2 loopfrontpos, Vector2 loopbackpos, Direction direction) : base(id,groep,direction, typeTrafficlight.Carlight)
        {
            this.loopfrontpos = loopfrontpos;
            this.loopbackpos = loopbackpos;

            loopFront = false;
            loopBack = false;

        }

        public CarTrafficLight(int id, char groep, Vector2 loopfrontpos, Vector2 loopbackpos, Direction direction, Vector2 turnposition,Drivedirection futuredirection):base(id, groep, direction, typeTrafficlight.Carlight)
        {
            this.loopfrontpos = loopfrontpos;
            this.loopbackpos = loopbackpos;

            this.turnposition = turnposition;
            this.futuredirection = futuredirection;

            loopFront = false;
            loopBack = false;

        }

        public override Info GetRoadInfo()
        {
            return new CarRoadInfo(loopFront,loopBack,false);

        }

        public override void SetLoop(List<Vehicle> vehicles)
        {
            foreach (Car car in vehicles)
            {
                if (car.onpoint(loopfrontpos) && loopFront == false)
                {
                    loopFront = true;
                    car.setwaitingtrafficlight(this);
                    Onfrontloop = car;
                    if(getDirection()==Direction.right|| getDirection() == Direction.left)
                    {
                        car.setturningpoint(turnposition,futuredirection);
                    }

                }
                else if (Onfrontloop != null && !Onfrontloop.onpoint(loopfrontpos))
                {
                    loopFront = false;
                    Onfrontloop = null;
                }

                if (car.onpoint(loopbackpos) && loopBack == false)
                {
                    loopBack = true;
                    Onbackloop = car;
                }
                else if (Onbackloop != null && !Onbackloop.onpoint(loopbackpos))
                {
                    loopBack = false;
                    Onbackloop = null;
                }

            }
        }

    }
}


