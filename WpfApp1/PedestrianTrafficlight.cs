using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class PedestrianTrafficlight : TrafficLight
    {
        public PedestrianTrafficlight(int id, char groep) : base(id, groep, Direction.straight, typeTrafficlight.PedestrianLight)
        {

        }

        public override string fullId()
        {
            return base.fullId()+ "Pedes";
        }


    }
}
