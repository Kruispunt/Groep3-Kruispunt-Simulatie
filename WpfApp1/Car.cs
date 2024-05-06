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



    }
}