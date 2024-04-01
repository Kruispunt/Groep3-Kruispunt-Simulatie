using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Numerics;

namespace WpfApp1
{
    public class Trafficlights
    {
        private int TickCounter;
        private int color;
        private int id;

        private char groep;

        private bool loopFront;
        private bool loopBack;

        private Rectangle box;
        private Car Onfrontloop;
        private Car Onbackloop;

        private Vector2 turnposition;
        private Vector2 loopfrontpos;//first loop point
        private Vector2 loopbackpos;//second loop point

        private Direction direction;
        private Drivedirection futuredirection;

        public Trafficlights(int id,char groep, Vector2 loopfrontpos, Vector2 loopbackpos, Direction direction)
        {
            this.id = id;
            this.groep = groep;
            this.direction = direction;

            this.loopfrontpos = loopfrontpos;
            this.loopbackpos = loopbackpos;

            color = 0;
            loopFront = false;
            loopBack = false;

            //this.m = new MessageOutRoot(m);
        }

        public Trafficlights(int id, char groep, Vector2 loopfrontpos, Vector2 loopbackpos, Direction direction, Vector2 turnposition,Drivedirection futuredirection)
        {
            this.id = id;
            this.groep = groep;
            this.direction = direction;
            this.futuredirection =futuredirection;

            this.loopfrontpos = loopfrontpos;
            this.loopbackpos = loopbackpos;
            this.turnposition = turnposition;

             color = 0;
             loopFront = false;
             loopBack = false;

             //this.m = new MessageOutRoot(m);
        }

        public string fullId() { return groep.ToString() + id.ToString(); }

        public int getid() { return id; }

        public char getgroep() { return groep; }

        public CarRoadInfo GetCarRoadInfo()
        {
            CarRoadInfo roadInfo = new CarRoadInfo();
            roadInfo.DetectFar = loopBack;
            roadInfo.DetectNear = loopFront;
            roadInfo.PrioCar = false; 
            return roadInfo;
        }

        public Direction getDirection() {  return this.direction; }

        //public MessageOutRoot GetMessage() { return this.m;}

        public int getcolor() { return color; }

        public void setrectangle(Rectangle box) { this.box = box; }

        public void setloops(List<Car> cars)
        {
            foreach (Car car in cars)
            {
                if (car.onpoint(loopfrontpos) && loopFront == false)
                {
                    loopFront = true;
                    car.setwaitingtrafficlight(this);
                    Onfrontloop = car;
                    if(direction==Direction.right||direction == Direction.left)
                    {
                        car.setturningpoint(turnposition,futuredirection);
                    }

                }
                if (car.onpoint(loopbackpos) && loopBack == false)
                {
                    loopBack = true;
                    Onbackloop = car;
                }

                if (Onfrontloop != null && !Onfrontloop.onpoint(loopfrontpos))
                {
                    loopFront = false;
                    Onfrontloop = null;
                }
                if (Onbackloop != null && !Onbackloop.onpoint(loopbackpos))
                {
                    loopBack = false;
                    Onbackloop = null;
                }

            }
        }

        public void setcolor(int color)
        {
            this.color = color;

        }

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

            if(TickCounter == 100)
            {
                if (color == 2)
                {
                    color = 0;
                }
                else
                {
                    color =2;
                }
                colorchange();
                TickCounter = 0;

            }


        }

    }
}

public enum Direction {straight, right,left }
