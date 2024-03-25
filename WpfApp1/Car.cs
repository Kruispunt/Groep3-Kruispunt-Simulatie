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
        private direction direction;
        private double width;
        private double height;
        private bool driving;
        
        public Car(int x, int y, Rectangle box, direction direction,double width,double height) {
            TickCounter = 0;
            position.X = x;
            position.Y = y;
            this.box = box;
            this.direction = direction;
            this.width = width;
            this.height = height;
            driving = true;
        }

        public Vector2 getposition()
        {
            return position;
        }

        public double getwidth() { return width; }

        public double getheight() { return height; }    

        public Rectangle GetRectangle()
        {
            return box;
        }

        public void Tick()
        {
            if (driving)
            {
                Move();
            }

        }

        public void Move()
        {
            if(direction == direction.North)
            {
                position.Y = position.Y - 1;
            }
            else if(direction == direction.South)
            {
                position.Y = position.Y + 1;
            }
            else if (direction == direction.West)
            {
                position.X = position.X - 1;
            }
            else if(direction == direction.East)
            {
                position.X = position.X + 1;
            }
        }

        public void CheckTrafficLight(Trafficlights trafficlights)
        {
            if (trafficlights.getcolor() == 0)
            {
                driving = false;
            }
            else
            {
                driving = true;
            }
        }

    }
}
public enum direction {North,South,West,East }