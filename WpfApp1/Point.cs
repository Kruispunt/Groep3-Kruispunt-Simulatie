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

        private Spawnpointtype spawnpoint;

        public Point(Vector2 point, Drivedirection Drivedirection, Spawnpointtype spawnpoint) {
            this.point = point; 
            this.Drivedirection = Drivedirection;
            this.spawnpoint = spawnpoint;
        }
        public Vector2 getpoint() { return point; }
        public Drivedirection GetDrivedirection() { return Drivedirection; }

        public Spawnpointtype getspawnpointtype() { return spawnpoint; }
    }

    public class Turnpoint:Point {

        private bool required;
        public Turnpoint(Vector2 point, Drivedirection Drivedirection, bool required, Spawnpointtype spawnpoint) : base(point, Drivedirection,spawnpoint)
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

        public Switchlanepoint(Vector2 point, Drivedirection Drivedirection, bool required, bool x, float endpoint, Spawnpointtype spawnpoint) : base(point, Drivedirection, required,spawnpoint)
        {
            this.x = x;
            this.endpoint = endpoint;
        }

        public bool getX() { return x; }
        public float getEnddestinatation() { return endpoint; }

    }
}
public enum Spawnpointtype {Car,Bus,Bicycle,Pedestrian}
