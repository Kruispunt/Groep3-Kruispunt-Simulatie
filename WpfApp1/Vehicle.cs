using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WpfApp1
{
    public class Vehicle
    {
        private Rectangle box;
        private Vector2 position;
        private Drivedirection direction;

        private float width;
        private float height;

        private float _width;
        private float _height;

        private bool moving;
        private int speed;


        public Vehicle(float x, float y, Rectangle box, Drivedirection direction, float width, float height, int speed) {
            position.X = x;
            position.Y = y;
            this.box = box;
            this.width = width;
            this._width = width;
            this._height = height;
            this.height = height;
            moving = true;
            this.direction = direction;
            this.speed = speed;
        }

        public virtual void setwaitingtrafficlight(TrafficLight trafficlights) {}

        public void setMoving(bool moving) { this.moving = moving; }

        public void setDriveDirection(Drivedirection drivedirection) { direction = drivedirection; }

        public Rectangle GetRectangle(){ return box; }

        public float GetWidth() { return width; }
        public float GetHeight() { return height; }

        public Vector2 getposition(){return position;}

        public Drivedirection getdirection() { return direction;}
        public void setposition(float x, float y){position.X = x; position.Y = y; }
        public void setposition(Vector2 pos){ position = pos; }

        public void setspeed(int speed) { this.speed = speed; }

        public virtual void CheckTrafficLight(TrafficLight trafficlights){}

        public virtual void Tick()
        {

            if (moving)
            {
                Move();
            }
        }
        public virtual void Move()
        {
            if (direction == Drivedirection.North)
            {
                setposition(getposition().X, getposition().Y - speed);
            }
            else if (direction == Drivedirection.South)
            {
                setposition(getposition().X, getposition().Y + speed);
            }
            else if (direction == Drivedirection.West)
            {
                setposition(getposition().X - speed, getposition().Y);
            }
            else if (direction == Drivedirection.East)
            {
                setposition(getposition().X + speed, getposition().Y);
            }
            else if(direction == Drivedirection.SouthWest)
            {
                setposition(getposition().X - speed, getposition().Y+speed);
            }
            else if (direction == Drivedirection.SouthEast)
            {
                setposition(getposition().X + speed, getposition().Y+speed);
            }
            else if (direction == Drivedirection.NorthWest)
            {
                setposition(getposition().X - speed, getposition().Y-speed);
            }
            else if (direction == Drivedirection.NorthEast)
            {
                setposition(getposition().X + speed, getposition().Y-speed);
            }
        }


        public void setrotation(Drivedirection drivedirection)
        {

            direction = drivedirection;
            if (direction == Drivedirection.North)
            {
                resize(_width, _height);
            }
            if (direction == Drivedirection.South)
            {
                resize(_width, _height);

            }
            if (direction == Drivedirection.East)
            {
                resize(_height, _width);

            }
            if (direction == Drivedirection.West)
            {
                resize(_height, _width);
            }
        }

        public bool onpoint(Vector2 point)
        {
            if (point.X > getposition().X - ((GetWidth() / 2.0)) && point.X < getposition().X + ((GetWidth() / 2.0)) && point.Y > getposition().Y - ((GetHeight() / 2.0)) && point.Y < getposition().Y + (GetHeight() / 2.0))
            {
                return true;
            }
            return false;
        }

        public bool closeby(Vector2 point,float outer)
        {

            if (getdirection() == Drivedirection.North || getdirection() == Drivedirection.South)
            {
                if (point.X > getposition().X - ((GetWidth() / 2.0) + outer) && point.X < getposition().X + ((GetWidth() / 2.0) + outer) && point.Y > getposition().Y - ((GetHeight() / 2.0) + GetHeight()) && point.Y < getposition().Y + ((GetHeight() / 2.0) + GetHeight()))
                {
                    return true;
                }
            }
            else if (getdirection() == Drivedirection.East || getdirection() == Drivedirection.West)
            {
                if (point.X > getposition().X - ((GetWidth() / 2.0) + GetWidth()) && point.X < getposition().X + ((GetWidth() / 2.0) + GetWidth()) && point.Y > getposition().Y - ((GetHeight() / 2.0) + outer) && point.Y < getposition().Y + ((GetHeight() / 2.0) + outer))
                {
                    return true;
                }
            }

            return false;
        }


        public void resize(float w, float h)
        {
            width = w;
            height = h;
            box.Height = height;
            box.Width = width;
        }
    }
}
public enum Drivedirection { North, South, West, East, NorthEast, NorthWest, SouthEast, SouthWest }