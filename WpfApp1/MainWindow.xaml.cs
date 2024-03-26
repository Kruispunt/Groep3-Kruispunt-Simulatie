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
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Tcp tcp;
        public DispatcherTimer timer = new DispatcherTimer();
        public List<Car> CarsOnScreen = new List<Car>();
        public List<Trafficlights> TrafficlightsOnScreen = new List<Trafficlights>();
        public List<MessageOut> tobesendmessages = new List<MessageOut>();
        public List<MessageIn> receivedmessages = new List<MessageIn>();
        public int ticker;
        public int connectionticker;
        public int screenleft=800;
        public int screenbottom=450;
        public string ipadress = "127.0.0.1";
        public MessageOutRoot message; //test

        public MainWindow()
        {
            InitializeComponent();
            timer.Interval =TimeSpan.FromMilliseconds(20);
            timer.Tick += Engine;
            timer.Start();
            Canvas.Focus();

            ticker = 0;
            connectionticker = 0;
            createnewtrafficlight(210,100,10,50, "A1");
            createnewtrafficlight(210,600,10,50, "D1");
            CreateNewCar(210, 0, direction.East);
            
            tcp = new Tcp();
            tcp.Connect(ipadress, 12345);

        }

        public void Engine(object sender, EventArgs e)
        {

            reconnect();

            messagesendandreceive();

            ticks();

            CarTrafficlightintersect();

            outabounds();

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

                Rect trafficbox = new Rect(Canvas.GetLeft(traffic.GetRectangle()), Canvas.GetTop(traffic.GetRectangle()), traffic.getwidth(), traffic.getheight());

                for (int i = 0; i < CarsOnScreen.Count(); i++)
                {
                    Rect carbox = new Rect(Canvas.GetLeft(CarsOnScreen[i].GetRectangle()), Canvas.GetTop(CarsOnScreen[i].GetRectangle()), CarsOnScreen[i].getwidth(), CarsOnScreen[i].getheight());
                    if (carbox.IntersectsWith(trafficbox))
                    {
                        CarsOnScreen[i].CheckTrafficLight(traffic);
                        if (traffic.gettrafficlightid() == "A1") { 

                            traffic.GetMessage().messageOut.detectielus = 1;
                            traffic.GetMessage().send = false;
                            message = traffic.GetMessage();
                        }
                    }
                    else if(traffic.gettrafficlightid() == "A1")
                    {
                        traffic.GetMessage().messageOut.detectielus = 0;
                        message = traffic.GetMessage();
                        traffic.GetMessage().send = false;
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
                Height = height,
                Width = width,
                Fill = Brushes.Red
            };

            Canvas.SetTop(newtraffic, y);
            Canvas.SetLeft(newtraffic, x);
            Canvas.Children.Add(newtraffic);

            MessageOut m = new MessageOut();
            m.trafficlightid = id;
            TrafficlightsOnScreen.Add(new Trafficlights(id,newtraffic, new Vector2(x,y),height, width, m));
        }

        #endregion

        #region messages

        public void messagesendandreceive()
        {
            ticker++;
            if (ticker == 20)
            {
                if (message == null)
                {
                    return;
                }

                if (message.send==false)
                {
                    test.Text = tcp.sendmessages(message.messageOut);

                    receivedmessages = tcp.receivemessages();
                    test.Text = receivedmessages.Count().ToString();
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
                    receivedmessages.Clear();
                    message.send = true;
                }
                ticker = 0;
            }
        }
        #endregion

        public void reconnect()
        {
            if (tcp.getconnected() == false)
            {
                connectionticker++;

                if (connectionticker == 5)
                {
                    tcp.Connect(ipadress, 12345);
                    connectionticker = 0;
                }

            }

        }
    }
}