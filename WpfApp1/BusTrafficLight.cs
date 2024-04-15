using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class BusTrafficLight : TrafficLight
    {
        private Bus busloop;

        private BusTrafficLight neighbourTrafficLight;
        private Drivedirection futuredirection;

        private Vector2 turnposition;
        private Vector2 looppos;

        public BusTrafficLight(int id, char groep, Direction direction, Vector2 looppos,Vector2 turnposition, Drivedirection futuredirection) : base(id, groep, direction, typeTrafficlight.Buslight)
        {
            this.looppos = looppos;
            this.futuredirection = futuredirection;
            this.turnposition = turnposition;
        }

        public override string fullId()
        {
            return base.fullId() + "Bus";
        }

        public void setneighbour(BusTrafficLight neighbourtrafficlight) { this.neighbourTrafficLight = neighbourtrafficlight; }
        public BusTrafficLight getneighbour() { return neighbourTrafficLight; }


        public int getbusline() { return busloop.getline(); }

        public override void SetLoop(List<Vehicle> vehicles)
        {
            List<Bus> busses = vehicles.OfType<Bus>().ToList();
            foreach (Bus bus in busses)
            {
                if (bus.onpoint(looppos) && busloop == null && bus.goleft()== getid())
                {
                    bus.setwaitingtrafficlight(this);
                    busloop = (Bus)bus;
                    if (getDirection() == Direction.right || getDirection() == Direction.left)
                    {
                        bus.setturningpoint(turnposition, futuredirection);
                    }

                }
                else if (busloop != null && !busloop.onpoint(looppos))
                {
                    busloop = null;
                }
            }
        }

    }
}
