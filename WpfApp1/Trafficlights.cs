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
        private Rectangle box;
        private string Id;
        private Vector2 position;
        private double height;
        private double width;
        private MessageOutRoot m;


        public Trafficlights(string Id, Rectangle box, Vector2 position, double height, double width, MessageOut m)
        {
            this.Id = Id;
            color = 0;
            this.box = box;
            this.position = position;
            this.height = height;
            this.width = width;
            this.m = new MessageOutRoot(m);
        }

        public string gettrafficlightid(){ return this.Id; }

        public Vector2 getposition(){ return this.position;}

        public double getheight() { return this.height;}
               
        public double getwidth() { return this.width;}

        public Rectangle GetRectangle() { return this.box;}

        public MessageOutRoot GetMessage() { return this.m;}

        public int getcolor() { return color; }

        public void setcolor(int color)
        {
            this.color = color;

        }

        public void Tick()
        {
            //colorchangetest();
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

            if(TickCounter == 50)
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
