using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1
{
    public class TrafficLight
    {
        private int TickCounter;
        private int color;
        private int id;
        private char group;
        private Info Info;

        private typeTrafficlight trafficType;
        private Direction direction;

        private Rectangle box;

        public TrafficLight(int id, char groep, Direction direction,typeTrafficlight trafficType)
        {

            this.id = id;
            this.group = groep;
            this.direction = direction;

            color = 0;
            this.trafficType = trafficType;
        }

        //gets
        public virtual string fullId() { return group.ToString() + id.ToString(); }

        public int getid() { return id; }

        public char getgroep() { return group; }

        public typeTrafficlight GettrafficType()
        {
            return trafficType;

        }

        public Direction getDirection() { return this.direction; }

        public int getcolor() { return color; }

        public virtual Info GetRoadInfo(){ return new Info(); }

        //sets

        public void setrectangle(Rectangle box) { this.box = box; }

        public void setcolor(int color)
        {
            this.color = color;

        }

        public virtual void SetLoop(List<Vehicle> vehicles)
        {

        }

        //others

        public void Tick()
        {
            colorchangetest();
            colorchange();

        }

        public void colorchange()
        {
            if (color == 2)
            {
                box.Fill = Brushes.Green;
            }
            else if (color == 1)
            {
                box.Fill = Brushes.Orange;
            }
            else
            {
                box.Fill = Brushes.Red;
            }

        }

        public void colorchangetest()
        {
            TickCounter++;

            if (TickCounter == 200)
            {
                if (color == 2)
                {
                    color = 0;
                }
                else
                {
                    color = 2;
                }
                colorchange();
                TickCounter = 0;

            }


        }

    }
}
public enum Direction {straight, right,left }
public enum typeTrafficlight { Carlight, Buslight, PedestrianLight, Cyclistlight }
