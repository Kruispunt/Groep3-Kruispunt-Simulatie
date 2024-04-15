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
    public class Car:Vehicle
    {
        private Drivedirection futuredir;
        private CarTrafficLight lightwaiting;
        private Vector2 turningpoint;
        private bool turning;
        
        public Car(float x, float y, Rectangle box, Drivedirection direction,float width,float height, int speed) :base(x,y,box,direction,width,height,speed)
        {
            setrotation(direction);
            turning = false;
        }
        
        public override void setwaitingtrafficlight(TrafficLight trafficlights) { lightwaiting = (CarTrafficLight)trafficlights; }

        public void setturningpoint(Vector2 turningpoint, Drivedirection futuredir) 
        { 
            this.turningpoint=turningpoint;
            this.futuredir = futuredir;
        }

        public override void Tick()
        {
            if (lightwaiting != null)
            {
                CheckTrafficLight(lightwaiting);
            }

            if (onpoint(turningpoint) == true && turning == false)
            {
                setrotation(futuredir);
                setposition(turningpoint);
                turning = true;
            }

            if(onpoint(turningpoint) == false && turning == true)
            {
                turning = false;
            }

            base.Tick();
        }

        public bool onpoint(Vector2 point)
        {
            if (point.X>getposition().X-((GetWidth()/2.0)) && point.X < getposition().X + ((GetWidth() / 2.0))&& point.Y > getposition().Y - ((GetHeight() / 2.0))&&point.Y < getposition().Y + (GetHeight() / 2.0))
            {
                return true;
            }
            return false;
        }

        public void CheckTrafficLight(CarTrafficLight trafficlights)
        {
            if (trafficlights.getcolor() == 0)
            {
                setMoving(false);
                lightwaiting = trafficlights;
            }
            else
            {
                setMoving(true);
                lightwaiting = null;
            }
        }

        public bool closeby(Vector2 point)
        {

            if (getdirection() == Drivedirection.North || getdirection() == Drivedirection.South)
            {
                if (point.X > getposition().X - ((GetWidth() / 2.0)+1) && point.X < getposition().X + ((GetWidth() / 2.0)+1) && point.Y > getposition().Y - ((GetHeight() / 2.0) + 15) && point.Y < getposition().Y + ((GetHeight() / 2.0) + 15))
                {
                    return true;
                }
            }
            else if (getdirection() == Drivedirection.East || getdirection() == Drivedirection.West)
            {
                if (point.X > getposition().X - ((GetWidth() / 2.0)+15) && point.X < getposition().X + ((GetWidth() / 2.0) + 15) && point.Y > getposition().Y - ((GetHeight() / 2.0)+1) && point.Y < getposition().Y + ((GetHeight() / 2.0)+1))
                {
                    return true;
                }
            }

            return false;
        }

    }
}