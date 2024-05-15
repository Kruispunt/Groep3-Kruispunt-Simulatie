using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace WpfApp1
{
    class Bycicle : Vehicle
    {
        private BicycleTrafficLight lightwaiting;
        private Turnpoint futurepoint;

        public Bycicle(float x, float y, Rectangle box, Drivedirection direction, float width, float height, int speed) : base(x, y, box, direction, width, height, speed)
        {

        }

        public override void setwaitingtrafficlight(TrafficLight trafficlights) { lightwaiting = (BicycleTrafficLight)trafficlights; }

        public override void Tick()
        {
            if (lightwaiting != null)
            {
                CheckTrafficLight(lightwaiting);
            }

            base.Tick();
        }

        public void checkturnpoints(List<Turnpoint> turnpoints)
        {

            foreach (Turnpoint turnpoint in turnpoints)
            {
                if (onpoint(turnpoint.getpoint()) == true)
                {
                    Random r = new Random();
                    if ((turnpoint.getRequired()== true||r.Next(100)<=turnpoint.getTurnpercentage())&& turnpoint.getincoming()==getdirection())
                    {
                        setDriveDirection(turnpoint.GetDrivedirection());
                    }

                }
            }
        }

        public void CheckTrafficLight(BicycleTrafficLight trafficlights)
        {
            if (trafficlights.getcolor() == 0)
            {
                setMoving(false);
                lightwaiting = trafficlights;
            }
            else
            {
                setMoving(true);
                lightwaiting = null;
            }
        }
    }
}
