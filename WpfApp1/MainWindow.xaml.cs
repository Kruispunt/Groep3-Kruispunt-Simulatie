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
        public MessageOut messageOut= new MessageOut();

        public List<Car> CarsOnScreen = new List<Car>();
        public List<Trafficlights> TrafficlightsOnScreen = new List<Trafficlights>();
        //public List<MessageOut> tobesendmessages = new List<MessageOut>();
        //public List<MessageIn> receivedmessages = new List<MessageIn>();

        public List<Spawnpoint>spawnpoints = new List<Spawnpoint>(); 

        public int ticker;
        public int connectionticker;

        public int spawncars;
        public int maxcars=4;
        public int screenleft=800;
        public int screenbottom=450;
        public string ipadress = "127.0.0.1";


        public MainWindow()
        {
            InitializeComponent();

            timer.Interval =TimeSpan.FromMilliseconds(20);
            timer.Tick += Engine;
            timer.Start();
            Canvas.Focus();

            trafficlights();
            Spawnpoints();

            ticker = 0;
            spawncars = 99;
            connectionticker = 0;
            
            //tcp = new Tcp();
            //tcp.Connect(ipadress, 12345);

        }

        public void Engine(object sender, EventArgs e)
        {
            updateMessage();
            if (spawncars >= 100 && CarsOnScreen.Count() < maxcars)
            {
                CreateNewCar();
                spawncars = 0;
            }
            else
            {
                spawncars++;
            }


            reconnect();

            messagesendandreceive();

            ticks();

            outabounds();

        }


        public void updateMessage()
        {
            for(int i = 0;i< TrafficlightsOnScreen.Count(); i++)
            {
                if (TrafficlightsOnScreen[i].getgroep()=='A') {
                    if (TrafficlightsOnScreen[i].getid() == 0)
                    {
                        messageOut.A.Cars[0] = TrafficlightsOnScreen[i].GetCarRoadInfo();
                    }
                    else if (TrafficlightsOnScreen[i].getid() == 1)
                    {
                        messageOut.A.Cars[1] = TrafficlightsOnScreen[i].GetCarRoadInfo();
                    }
                    else if (TrafficlightsOnScreen[i].getid() == 2)
                    {
                        messageOut.A.Cars[2] = TrafficlightsOnScreen[i].GetCarRoadInfo();
                    }
                    else if (TrafficlightsOnScreen[i].getid() == 3)
                    {
                        messageOut.A.Cars[3] = TrafficlightsOnScreen[i].GetCarRoadInfo();
                    }
                }
            }
        }

        public void ticks()
        {
            for (int i = 0; i < CarsOnScreen.Count(); i++)
            {
                CarsOnScreen[i].Tick();
                //add collision detect
                Canvas.SetTop(CarsOnScreen[i].GetRectangle(),CarsOnScreen[i].getposition().Y);
                Canvas.SetLeft(CarsOnScreen[i].GetRectangle(),CarsOnScreen[i].getposition().X);
            }

            for (int i = 0; i < TrafficlightsOnScreen.Count(); i++)
            {
                TrafficlightsOnScreen[i].Tick();
                TrafficlightsOnScreen[i].setloops(CarsOnScreen);
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

        public void Spawnpoints()
        {
            spawnpoints.Add(new Spawnpoint(new Vector2(0, 200),Drivedirection.East));
            spawnpoints.Add(new Spawnpoint(new Vector2(0, 211),Drivedirection.East));
            spawnpoints.Add(new Spawnpoint(new Vector2(0, 222),Drivedirection.East));
            spawnpoints.Add(new Spawnpoint(new Vector2(0, 233),Drivedirection.East));
        }

        public void trafficlights()
        {
            TrafficlightsOnScreen.Add(new Trafficlights(0, 'A', new Vector2(90, 200), new Vector2(80, 200), Direction.straight));//A1
            TrafficlightsOnScreen.Add(new Trafficlights(1, 'A', new Vector2(90, 211), new Vector2(80, 211), Direction.straight));//A2
            TrafficlightsOnScreen.Add(new Trafficlights(2, 'A', new Vector2(90, 222), new Vector2(80, 222), Direction.right, new Vector2(117, 222), Drivedirection.South));//A3
            TrafficlightsOnScreen.Add(new Trafficlights(3, 'A', new Vector2(90, 233), new Vector2(80, 233), Direction.right, new Vector2(106, 233), Drivedirection.South));//A4


            foreach (Rectangle x in Canvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "trafficlight")
                {
                    foreach (Trafficlights trafficlights in TrafficlightsOnScreen)
                    {
                        if ((string)x.Name == trafficlights.fullId())
                        {
                            trafficlights.setrectangle(x);
                        }
                    }
                }
            }

        }

        public void CreateNewCar()
        {
            Random r = new Random();
            int i = r.Next(4);
            Spawnpoint spawnpoint = spawnpoints[i];

            Rectangle newAuto = new Rectangle
            {
                Tag = "Auto",
                Height = 15,
                Width = 10,
                Fill = Brushes.Black
            };
            
            Canvas.SetTop(newAuto, spawnpoint.getpoint().Y);
            Canvas.SetLeft(newAuto, spawnpoint.getpoint().X);
            Canvas.Children.Add(newAuto);

            CarsOnScreen.Add(new Car(spawnpoint.getpoint().X, spawnpoint.getpoint().Y, newAuto,spawnpoint.GetDrivedirection(),20,30));
        }

        #region messages


        public void messagesendandreceive()
        {
            ticker++;
            if (ticker == 20)
            {
                if (messageOut == null)
                {
                    return;
                }


                test.Text = tcp.sendmessages(messageOut);

                /*
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
                */
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