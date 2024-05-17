using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace WpfApp1
{
    public class Bycicle : Vehicle
    {
        private BicycleTrafficLight lightwaiting;
        private int justturned;

        public Bycicle(float x, float y, Rectangle box, Drivedirection direction, float width, float height, float speed) : base(x, y, box, direction, width, height, speed)
        {
            justturned = 0;
        }

        public override void setwaitingtrafficlight(TrafficLight trafficlights) { lightwaiting = (BicycleTrafficLight)trafficlights; }

        public override void Tick()
        {
            justturned--;
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
                if (onpoint(turnpoint.getpoint()) == true && turnpoint.getspawnpointtype()==Spawnpointtype.Bicycle)
                {
                    Random r = new Random();
                    if ((turnpoint.getRequired()== true||(r.Next(100)<=turnpoint.getTurnpercentage()&&justturned<=0))&& turnpoint.getincoming()==getdirection())
                    {
                        setDriveDirection(turnpoint.GetDrivedirection());
                        setposition(turnpoint.getpoint());
                        justturned = 50;
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
