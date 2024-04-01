using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Spawnpoint
    {
        private Vector2 point;

        private Drivedirection Drivedirection;

        public Spawnpoint(Vector2 point, Drivedirection Drivedirection) {
            this.point = point; 
            this.Drivedirection = Drivedirection;
        }
        public Vector2 getpoint() { return point; }
        public Drivedirection GetDrivedirection() { return Drivedirection; }
    }
}
