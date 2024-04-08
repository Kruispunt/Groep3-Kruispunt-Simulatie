using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Point
    {
        private Vector2 point;

        private Drivedirection Drivedirection;

        public Point(Vector2 point, Drivedirection Drivedirection) {
            this.point = point; 
            this.Drivedirection = Drivedirection;
        }
        public Vector2 getpoint() { return point; }
        public Drivedirection GetDrivedirection() { return Drivedirection; }
    }

    public class Turnpoint:Point {

        private bool required;
        public Turnpoint(Vector2 point, Drivedirection Drivedirection, bool required):base(point, Drivedirection)
        {
            this.required = required;
        }

        public bool getRequired()
        {
            return required;
        }
    }

    public class Switchlanepoint : Turnpoint
    {
        private bool x;
        private float endpoint;

        public Switchlanepoint(Vector2 point, Drivedirection Drivedirection, bool required, bool x, float endpoint) : base(point, Drivedirection, required)
        {
            this.x = x;
            this.endpoint = endpoint;
        }

        public bool getX() { return x; }
        public float getEnddestinatation() { return endpoint; }

    }
}
