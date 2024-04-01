using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Shapes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfApp1
{
    public class Car
    {
        private int TickCounter;
        private Rectangle box;
        private Vector2 position;
        private Drivedirection direction;
        private Drivedirection futuredir;
        private double width;
        private double height;
        private bool driving;
        private Trafficlights lightwaiting;
        private Vector2 turningpoint;
        
        public Car(float x, float y, Rectangle box, Drivedirection direction,double width,double height) {
            TickCounter = 0;
            position.X = x;
            position.Y = y;
            this.box = box;
            this.width = width;
            this.height = height;
            driving = true;
            setrotation(direction);
        }

        public void setrotation(Drivedirection drivedirection)
        {
            direction=drivedirection;
            if(direction==Drivedirection.North || direction == Drivedirection.South)
            {
                resize(10, 15);
            }
            if (direction == Drivedirection.East || direction == Drivedirection.West)
            {
                resize(15, 10);
            }
        }

        public void setwaitingtrafficlight(Trafficlights trafficlights) { lightwaiting = trafficlights; }

        public void setturningpoint(Vector2 turningpoint, Drivedirection futuredir) 
        { 
            this.turningpoint=turningpoint;
            this.futuredir = futuredir;
        }

        public Rectangle GetRectangle()
        {
            return box;
        }

        public Vector2 getposition()
        {
            return position;
        }

        public void resize(double w, double h) 
        {
            width = w;
            height = h;
            box.Height = height;
            box.Width = width;
        
        }

        public void Tick()
        {
            if (lightwaiting != null)
            {
                CheckTrafficLight(lightwaiting);
            }

            if (driving)
            {
                Move();
            }
        }

        public void Move()
        {
            if (onpoint(turningpoint) == true)
            {
                setrotation(futuredir);

            }
            if(direction == Drivedirection.North)
            {
                position.Y = position.Y - 1;
            }
            else if(direction == Drivedirection.South)
            {
                position.Y = position.Y + 1;
            }
            else if (direction == Drivedirection.West)
            {
                position.X = position.X - 1;
            }
            else if(direction == Drivedirection.East)
            {
                position.X = position.X + 1;
            }
        }
        //add turn movements

        public bool onpoint(Vector2 point)
        {
            if (point.X>position.X-((width/2.0)) && point.X < position.X + ((width / 2.0))&& point.Y > position.Y - ((height / 2.0))&&point.Y < position.Y + (height / 2.0))
            {
                return true;
            }
            return false;
        }

        public void CheckTrafficLight(Trafficlights trafficlights)
        {
            if (trafficlights.getcolor() == 0)
            {
                driving = false;
                lightwaiting = trafficlights;
            }
            else
            {
                driving = true;
                lightwaiting = null;
            }
        }

    }
}
public enum Drivedirection {North,South,West,East }