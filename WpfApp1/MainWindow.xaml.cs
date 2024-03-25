using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Numerics;
using Accessibility;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int maxcar = 3;
        public Tcp tcp;
        public DispatcherTimer timer = new DispatcherTimer();
        public List<Car> CarsOnScreen = new List<Car>();
        public List<Trafficlights> TrafficlightsOnScreen = new List<Trafficlights>();
        public List<MessageOut> tobesendmessages = new List<MessageOut>();
        public List<MessageIn> receivedmessages = new List<MessageIn>();
        public int ticker;
        public int Carticker;
        public int connectionticker;
        public int screenleft=800;
        public int screenbottom=450;

        public MainWindow()
        {
            InitializeComponent();
            timer.Interval =TimeSpan.FromMilliseconds(20);
            timer.Tick += Engine;
            timer.Start();
            Canvas.Focus();

            ticker = 0;
            Carticker = 0;
            connectionticker = 0;
            createnewtrafficlight(210,100,30,30, "A1");
            createnewtrafficlight(210,600,30,30, "D1");
            CreateNewCar(210, 0, direction.East);
            
            tcp = new Tcp();
            tcp.Connect("127.0.0.1", 12345);

        }

        public void Engine(object sender, EventArgs e)
        {
            Carticker++;
            if (Carticker == 200 && CarsOnScreen.Count()<maxcar)
            {
                CreateNewCar(210, 0, direction.East);
                Carticker = 0;
            }

            ticks();

            CarTrafficlightintersect();

            outabounds();

            reconnect();

            messagesend();

            messagereceive();
        }

        public void ticks()
        {
            for (int i = 0; i < CarsOnScreen.Count(); i++)
            {
                CarsOnScreen[i].Tick();
                Canvas.SetTop(CarsOnScreen[i].GetRectangle(),CarsOnScreen[i].getposition().Y);
                Canvas.SetLeft(CarsOnScreen[i].GetRectangle(),CarsOnScreen[i].getposition().X);
            }

            for (int i = 0; i < TrafficlightsOnScreen.Count(); i++)
            {
                TrafficlightsOnScreen[i].Tick();
            }
        }

        public void CarTrafficlightintersect()
        {
            foreach (Trafficlights traffic in TrafficlightsOnScreen)
            {
                Rect trafficbox = new Rect(Canvas.GetTop(traffic.GetRectangle()), Canvas.GetLeft(traffic.GetRectangle()), traffic.getwidth(), traffic.getheight());

                for (int i = 0; i < CarsOnScreen.Count(); i++)
                {
                    Rect carbox = new Rect(Canvas.GetTop(CarsOnScreen[i].GetRectangle()), Canvas.GetLeft(CarsOnScreen[i].GetRectangle()), CarsOnScreen[i].getwidth(), CarsOnScreen[i].getheight());
                    if (carbox.IntersectsWith(trafficbox))
                    {
                        CarsOnScreen[i].CheckTrafficLight(traffic);
                        traffic.GetMessage().detectielus = 1;
                        if (tobesendmessages.Contains(traffic.GetMessage()) == false)
                        {
                            tobesendmessages.Add(traffic.GetMessage());
                        }
                    }
                }

            }
        }


        public void outabounds()
        {
            for(int i = 0; i < CarsOnScreen.Count; i++)
            {
                if (CarsOnScreen[i].getposition().X < 0 || CarsOnScreen[i].getposition().X > screenleft || CarsOnScreen[i].getposition().Y < 0 || CarsOnScreen[i].getposition().Y >screenbottom)
                {
                    Canvas.Children.Remove(CarsOnScreen[i].GetRectangle());
                    CarsOnScreen.Remove(CarsOnScreen[i]);
                }
            }
        }


        #region create
        public void CreateNewCar( int y, int x, direction dir)
        {
            Rectangle newAuto = new Rectangle
            {
                Tag = "Auto",
                Height = 30,
                Width = 20,
                Fill = Brushes.Black
            };
            
            Canvas.SetTop(newAuto, y);
            Canvas.SetLeft(newAuto, x);
            Canvas.Children.Add(newAuto);

            CarsOnScreen.Add(new Car(x,y,newAuto,dir,20,30));
        }

        public void createnewtrafficlight(int y, int x, int width,int height, string id)
        {
            Rectangle newtraffic = new Rectangle
            {
                Tag = "TrafficLight",
                Height = width,
                Width = height,
                Fill = Brushes.Red
            };

            Canvas.SetTop(newtraffic, y);
            Canvas.SetLeft(newtraffic, x);
            Canvas.Children.Add(newtraffic);

            MessageOut m = new MessageOut();
            m.trafficlightid = id;
            TrafficlightsOnScreen.Add(new Trafficlights(id,newtraffic, new Vector2(x,y),width,height,m));
        }

        #endregion


        #region messages
        public void messagereceive()
        {
            receivedmessages = tcp.receivemessages();
            foreach (MessageIn messageIn in receivedmessages)
            {
                foreach (Trafficlights trafficlights in TrafficlightsOnScreen)
                {
                    if (messageIn.trafficlightid == trafficlights.gettrafficlightid())
                    {
                        trafficlights.setcolor(messageIn.color);
                    }
                }
            }
        }

        public void messagesend()
        {
            ticker++;
            if (ticker == 5)
            {
                test.Text = tcp.sendmessages(tobesendmessages);
                tobesendmessages.Clear();
                ticker = 0;
            }

        }
        #endregion

        public void reconnect()
        {
            if (tcp.getconnected() == false)
            {
                connectionticker++;

                if (connectionticker == 100)
                {
                    tcp.Connect("127.0.0.1", 12345);
                }
            }

        }
    }
}