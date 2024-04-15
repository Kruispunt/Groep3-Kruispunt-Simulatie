using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace WpfApp1
{
    class Bus : Car
    {
        private int line;
        private BusTrafficLight lightwaiting;

        public Bus(float x, float y, Rectangle box, Drivedirection direction, float width, float height, int speed, int line) : base(x, y, box, direction, width, height, speed)
        {
            this.line = line;
        }

        public int getline() { return line; }

        public override void CheckTrafficLight(TrafficLight trafficlights)
        {
            if (trafficlights.getcolor() == 0)
            {
                setMoving(false);
                lightwaiting = (BusTrafficLight)trafficlights;
            }
            else
            {
                setMoving(true);
                lightwaiting = null;
            }
        }

        public void CheckBusLight(BusTrafficLight trafficlights)
        {
            if (trafficlights.getcolor() == 0)
            {
                if (goleft()==trafficlights.getid())
                {
                    setMoving(false);
                    lightwaiting = trafficlights;
                }
                else
                {
                    CheckBusLight(trafficlights.getneighbour());
                }
            }
            else
            {
                setMoving(true);
                lightwaiting = null;
            }
        }

        public override void Tick()
        {
            if (lightwaiting != null)
            {
                CheckBusLight(lightwaiting);
            }

            base.Tick();
        }

        public int goleft()
        {
            return line%2;
        }

        public void setwaitingtrafficlight(BusTrafficLight trafficlights) { lightwaiting = trafficlights; }

    }
}
