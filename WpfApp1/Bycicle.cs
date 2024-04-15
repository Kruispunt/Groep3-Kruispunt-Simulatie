using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace WpfApp1
{
    class Bycicle : Vehicle
    {
        public Bycicle(float x, float y, Rectangle box, Drivedirection direction, float width, float height, int speed) : base(x, y, box, direction, width, height, speed)
        {

        }
    }
}
