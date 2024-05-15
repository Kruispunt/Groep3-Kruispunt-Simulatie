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
        private Drivedirection incoming;
        private Turnpoint neighbour;
        private int turnpercentage;
        public Turnpoint(Vector2 point, Drivedirection Drivedirection, bool required, Spawnpointtype spawnpoint, Drivedirection incoming, int turnpercentage) : base(point, Drivedirection, spawnpoint)
        {
            this.required = required;
            this.incoming = incoming;
            this.turnpercentage = turnpercentage;
        }

        public bool getRequired()
        {
            return required;
        }

        public void setNeighbour(Turnpoint neighbour) { 
            this.neighbour = neighbour;  
        }

        public int getTurnpercentage() { return turnpercentage; }

        public Turnpoint getNeighbour() { return neighbour; }

        public Drivedirection getincoming() { return incoming; }
    }

    public class Switchlanepoint : Turnpoint
    {
        private bool x;
        private float endpoint;

        public Switchlanepoint(Vector2 point, Drivedirection Drivedirection, bool required, bool x, float endpoint, Spawnpointtype spawnpoint, Drivedirection incoming, int turnpercentage) : base(point, Drivedirection, required,spawnpoint, incoming, turnpercentage)
        {
            this.x = x;
            this.endpoint = endpoint;
        }

        public bool getX() { return x; }
        public float getEnddestinatation() { return endpoint; }

    }
}
public enum Spawnpointtype {Car,Bus,Bicycle,Pedestrian}
