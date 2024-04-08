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
using System.Windows.Media.Animation;
using System.Diagnostics;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Tcp tcp;
        public DispatcherTimer timer = new DispatcherTimer();
        public Mainmessage message1 = new Mainmessage();
        public MainMessageIn messagein= new MainMessageIn();

        public List<Vehicle> vehicles = new List<Vehicle>();
        public List<TrafficLight> TrafficlightsOnScreen = new List<TrafficLight>();

        public List<Point>spawnpoints = new List<Point>(); 
        public List<Turnpoint> turnpoints = new List<Turnpoint>();
        public List<Switchlanepoint> switchlanepoints = new List<Switchlanepoint>();
        public Rectangle objtets;

        public int ticker;
        public int connectionticker;

        public int spawncars;
        public int maxcars=100;
        public int screenleft=1600;
        public int screenbottom=900;
        public string ipadress = "127.0.0.1";
        public int carspeed = 4;


        public MainWindow()
        {
            InitializeComponent();



            timer.Interval =TimeSpan.FromMilliseconds(1);
            timer.Tick += Engine;
            timer.Start();
            Canvas.Focus();

            trafficlights();
            PointSet();

            ticker = 0;
            spawncars = 9;
            connectionticker = 0;

            tcp = new Tcp();
            tcp.Connect(ipadress, 12345);
            

        }

        public void Engine(object sender, EventArgs e)
        {

            if (spawncars >= 10 && vehicles.Count() < maxcars)
            {
                CreateNewCar();
                spawncars = 0;
            }
            else
            {
                spawncars++;
            }

            reconnect();

            ticks();
            carcollision();

            outabounds();


            updateMessage();

            messagesendandreceive();
        }

        public void updateMessage()
        {
            for(int i = 0;i< TrafficlightsOnScreen.Count(); i++)
            {
                //todo add pedestrians and add cyclists

                if (TrafficlightsOnScreen[i].GettrafficType() == typeTrafficlight.Carlight)
                {
                    switch (TrafficlightsOnScreen[i].getgroep())
                    {
                        case 'A':
                            message1.messageOut1.A.Cars[TrafficlightsOnScreen[i].getid()] = (CarRoadInfo)TrafficlightsOnScreen[i].GetRoadInfo();
                            break;                                                                                      
                        case 'B':                                                                                       
                            message1.messageOut1.B.Cars[TrafficlightsOnScreen[i].getid()] = (CarRoadInfo)TrafficlightsOnScreen[i].GetRoadInfo();
                            break;                                                                                      
                        case 'C':                                                                                       
                            message1.messageOut1.C.Cars[TrafficlightsOnScreen[i].getid()] = (CarRoadInfo)TrafficlightsOnScreen[i].GetRoadInfo();
                            break;                                                                                      
                        case 'D':                                                                                       
                            message1.messageOut2.D.Cars[TrafficlightsOnScreen[i].getid()] = (CarRoadInfo)TrafficlightsOnScreen[i].GetRoadInfo();
                            break;                                                                                      
                        case 'E':                                                                                       
                            message1.messageOut2.E.Cars[TrafficlightsOnScreen[i].getid()] = (CarRoadInfo)TrafficlightsOnScreen[i].GetRoadInfo();
                            break;                                                                                      
                        case 'F':                                                                                       
                            message1.messageOut2.F.Cars[TrafficlightsOnScreen[i].getid()] = (CarRoadInfo)TrafficlightsOnScreen[i].GetRoadInfo();
                            break;
                    }
                }

                if (TrafficlightsOnScreen[i].GettrafficType() == typeTrafficlight.Cyclistlight)
                {
                    switch (TrafficlightsOnScreen[i].getgroep())
                    {
                        case 'A':
                            message1.messageOut1.A.Cyclists[TrafficlightsOnScreen[i].getid()] = (CyclistRoadInfo)TrafficlightsOnScreen[i].GetRoadInfo();
                            break;
                        case 'B':
                            message1.messageOut1.B.Cyclists[TrafficlightsOnScreen[i].getid()] = (CyclistRoadInfo)TrafficlightsOnScreen[i].GetRoadInfo();
                            break;
                        case 'E':
                            message1.messageOut2.E.Cyclists[TrafficlightsOnScreen[i].getid()] = (CyclistRoadInfo)TrafficlightsOnScreen[i].GetRoadInfo();
                            break;
                        case 'F':
                            message1.messageOut2.F.Cyclists[TrafficlightsOnScreen[i].getid()] = (CyclistRoadInfo)TrafficlightsOnScreen[i].GetRoadInfo();
                            break;
                    }
                }
            }
        }

        public void ticks()
        {
            for (int i = 0; i < vehicles.Count(); i++)
            {
                vehicles[i].Tick();
                //add collision detect
                Canvas.SetTop(vehicles[i].GetRectangle(),vehicles[i].getposition().Y);
                Canvas.SetLeft(vehicles[i].GetRectangle(),vehicles[i].getposition().X);
            }

            for (int i = 0; i < TrafficlightsOnScreen.Count(); i++)
            {
                TrafficlightsOnScreen[i].Tick();
                TrafficlightsOnScreen[i].SetLoop(vehicles);
            }
        }

        public void outabounds()
        {
            for(int i = 0; i < vehicles.Count; i++)
            {
                if (vehicles[i].getposition().X < 0 || vehicles[i].getposition().X > screenleft || vehicles[i].getposition().Y < 0 || vehicles[i].getposition().Y >screenbottom)
                {
                    Canvas.Children.Remove(vehicles[i].GetRectangle());
                    vehicles.Remove(vehicles[i]);
                }
            }
        }

        public void carcollision()
        {
            foreach (Car car in vehicles)
            {
                foreach(Car car1 in vehicles)
                {
                    if (car == car1)
                    {
                        break;
                    }
                    else
                    {
                        if (car.closeby(car1.getposition()))
                        {
                            car.setMoving(false);
                            break;
                        }
                        car.setMoving(true);
                    }
                    
                }
            }
        }

        public void PointSet()
        {
            //-----------------------SpawnPoints------------------------------------
            //a points
            spawnpoints.Add(new Point(new Vector2(0, 400),Drivedirection.East));
            spawnpoints.Add(new Point(new Vector2(0, 411),Drivedirection.East));
            spawnpoints.Add(new Point(new Vector2(0, 422),Drivedirection.East));
            spawnpoints.Add(new Point(new Vector2(0, 433),Drivedirection.East));
                                
            //b points          
            spawnpoints.Add(new Point(new Vector2(140, 900),Drivedirection.North));
            spawnpoints.Add(new Point(new Vector2(151, 900),Drivedirection.North));
            spawnpoints.Add(new Point(new Vector2(162, 900),Drivedirection.North));
            spawnpoints.Add(new Point(new Vector2(173, 900),Drivedirection.North));
                                
            //F points          
            spawnpoints.Add(new Point(new Vector2(1600, 381), Drivedirection.West));
            spawnpoints.Add(new Point(new Vector2(1600, 370), Drivedirection.West));
            spawnpoints.Add(new Point(new Vector2(1600, 359), Drivedirection.West));
            spawnpoints.Add(new Point(new Vector2(1600, 348), Drivedirection.West));
                                
            //e points         
            spawnpoints.Add(new Point(new Vector2(922, 0), Drivedirection.South));
            spawnpoints.Add(new Point(new Vector2(911, 0), Drivedirection.South));
            spawnpoints.Add(new Point(new Vector2(900, 0), Drivedirection.South));

            //--------------------------turnpoints---------------------------------

            //--------------------------Changelanepoints---------------------------------
        }

        public void trafficlights()
        {
            TrafficlightsOnScreen.Add(new CarTrafficLight(0, 'A', new Vector2(50, 400), new Vector2(25, 400), Direction.straight));//A0
            TrafficlightsOnScreen.Add(new CarTrafficLight(1, 'A', new Vector2(50, 411), new Vector2(25, 411), Direction.straight));//A1
            TrafficlightsOnScreen.Add(new CarTrafficLight(2, 'A', new Vector2(50, 422), new Vector2(25, 422), Direction.right, new Vector2(111, 422), Drivedirection.South));//A2
            TrafficlightsOnScreen.Add(new CarTrafficLight(3, 'A', new Vector2(50, 433), new Vector2(25, 433), Direction.right, new Vector2(100, 433), Drivedirection.South));//A3

            TrafficlightsOnScreen.Add(new CarTrafficLight(0, 'B', new Vector2(140, 480), new Vector2(140, 505), Direction.left,  new Vector2(140, 359), Drivedirection.West));//B0 
            TrafficlightsOnScreen.Add(new CarTrafficLight(1, 'B', new Vector2(151, 480), new Vector2(151, 505), Direction.left,  new Vector2(151, 348), Drivedirection.West));//B1 
            TrafficlightsOnScreen.Add(new CarTrafficLight(2, 'B', new Vector2(162, 480), new Vector2(162, 505), Direction.right, new Vector2(162, 400), Drivedirection.East));//B2 
            TrafficlightsOnScreen.Add(new CarTrafficLight(3, 'B', new Vector2(173, 480), new Vector2(173, 505), Direction.right, new Vector2(173, 411), Drivedirection.East));//B3 

            TrafficlightsOnScreen.Add(new CarTrafficLight(0, 'C', new Vector2(185, 381), new Vector2(210, 381), Direction.left, new Vector2(104, 381), Drivedirection.South));//C0
            TrafficlightsOnScreen.Add(new CarTrafficLight(1, 'C', new Vector2(185, 370), new Vector2(210, 370), Direction.left, new Vector2(93, 370), Drivedirection.South));//C1
            TrafficlightsOnScreen.Add(new CarTrafficLight(2, 'C', new Vector2(185, 359), new Vector2(210, 359), Direction.straight)); //C2 
            TrafficlightsOnScreen.Add(new CarTrafficLight(3, 'C', new Vector2(185, 348), new Vector2(210, 348), Direction.straight)); //C3 


            TrafficlightsOnScreen.Add(new CarTrafficLight(0, 'D', new Vector2(890, 400), new Vector2(865, 400), Direction.left, new Vector2(955,400),Drivedirection.North));//D0
            TrafficlightsOnScreen.Add(new CarTrafficLight(1, 'D', new Vector2(890, 411), new Vector2(865, 411), Direction.left, new Vector2(966,411),Drivedirection.North));//D1
            TrafficlightsOnScreen.Add(new CarTrafficLight(2, 'D', new Vector2(890, 422), new Vector2(865, 422), Direction.straight));//D2
            TrafficlightsOnScreen.Add(new CarTrafficLight(3, 'D', new Vector2(890, 433), new Vector2(865, 433), Direction.straight));//D3
                                                                                                                                     
            TrafficlightsOnScreen.Add(new CarTrafficLight(0, 'E', new Vector2(922, 290), new Vector2(922, 265), Direction.left, new Vector2(922, 433), Drivedirection.East));//E1 
            TrafficlightsOnScreen.Add(new CarTrafficLight(1, 'E', new Vector2(911, 290), new Vector2(911, 265), Direction.right, new Vector2(911, 359), Drivedirection.West));//E2
            TrafficlightsOnScreen.Add(new CarTrafficLight(2, 'E', new Vector2(900, 290), new Vector2(900, 265), Direction.right, new Vector2(900, 348), Drivedirection.West));//E3

            TrafficlightsOnScreen.Add(new CarTrafficLight(0, 'F', new Vector2(1010, 381), new Vector2(1035, 381), Direction.straight));//F0
            TrafficlightsOnScreen.Add(new CarTrafficLight(1, 'F', new Vector2(1010, 370), new Vector2(1035, 370), Direction.straight));//F1
            TrafficlightsOnScreen.Add(new CarTrafficLight(2, 'F', new Vector2(1010, 359), new Vector2(1035, 359), Direction.right, new Vector2(966, 359), Drivedirection.North));//F2
            TrafficlightsOnScreen.Add(new CarTrafficLight(3, 'F', new Vector2(1010, 348), new Vector2(1035, 348), Direction.right, new Vector2(955, 348), Drivedirection.North));//F3



            //-------------------------add cyclist lights--------------------- //todo change loop points
            TrafficlightsOnScreen.Add(new BicycleTrafficLight(0, 'A', new Vector2(922, 290)));//A0 
            TrafficlightsOnScreen.Add(new BicycleTrafficLight(1, 'A', new Vector2(922, 290)));//A1 

            TrafficlightsOnScreen.Add(new BicycleTrafficLight(0, 'B', new Vector2(922, 290)));//A1 
            TrafficlightsOnScreen.Add(new BicycleTrafficLight(1, 'B', new Vector2(922, 290)));//A1 

            TrafficlightsOnScreen.Add(new BicycleTrafficLight(0, 'E', new Vector2(922, 290)));//A1 
            TrafficlightsOnScreen.Add(new BicycleTrafficLight(1, 'E', new Vector2(922, 290)));//A1 

            TrafficlightsOnScreen.Add(new BicycleTrafficLight(0, 'F', new Vector2(922, 290)));//A1 
            TrafficlightsOnScreen.Add(new BicycleTrafficLight(1, 'F', new Vector2(922, 290)));//A1 

            foreach (Rectangle x in Canvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "trafficlight")
                {
                    foreach (TrafficLight trafficlights in TrafficlightsOnScreen)
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
            int i = r.Next(spawnpoints.Count());
            Point spawnpoint = spawnpoints[i];

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

            vehicles.Add(new Car(spawnpoint.getpoint().X, spawnpoint.getpoint().Y, newAuto,spawnpoint.GetDrivedirection(),10,15,carspeed));
        }

        public void messagesendandreceive()
        {
            ticker++;
            if (ticker == 200&&tcp.getconnected()==true)
            {
                if (message1 == null)
                {
                    return;
                }

                test.Text = tcp.sendmessages(message1);

                messagein = tcp.receivemessages();

                if(messagein.messageIn1!=null && messagein.messageIn2 != null)
                {
                    for (int i = 0; i < TrafficlightsOnScreen.Count(); i++)
                    {
                        if (TrafficlightsOnScreen[i].GettrafficType() == typeTrafficlight.Carlight)
                        {
                            if (TrafficlightsOnScreen[i].getgroep() == 'A')
                            {
                                TrafficlightsOnScreen[i].setcolor(messagein.messageIn1.A.Cars[TrafficlightsOnScreen[i].getid()]);
                            }
                            if (TrafficlightsOnScreen[i].getgroep() == 'B')
                            {
                                TrafficlightsOnScreen[i].setcolor(messagein.messageIn1.B.Cars[TrafficlightsOnScreen[i].getid()]);
                            }
                            if (TrafficlightsOnScreen[i].getgroep() == 'C')
                            {
                                TrafficlightsOnScreen[i].setcolor(messagein.messageIn1.C.Cars[TrafficlightsOnScreen[i].getid()]);
                            }
                            if (TrafficlightsOnScreen[i].getgroep() == 'D')
                            {
                                TrafficlightsOnScreen[i].setcolor(messagein.messageIn2.D.Cars[TrafficlightsOnScreen[i].getid()]);
                            }
                            if (TrafficlightsOnScreen[i].getgroep() == 'E')
                            {
                                TrafficlightsOnScreen[i].setcolor(messagein.messageIn2.E.Cars[TrafficlightsOnScreen[i].getid()]);
                            }
                            if (TrafficlightsOnScreen[i].getgroep() == 'F')
                            {
                                TrafficlightsOnScreen[i].setcolor(messagein.messageIn2.F.Cars[TrafficlightsOnScreen[i].getid()]);
                            }
                        }
                        if (TrafficlightsOnScreen[i].GettrafficType() == typeTrafficlight.Cyclistlight)
                        {
                            if (TrafficlightsOnScreen[i].getgroep() == 'A')
                            {
                                TrafficlightsOnScreen[i].setcolor(messagein.messageIn1.A.Cyclists[TrafficlightsOnScreen[i].getid()]);
                            }
                            if (TrafficlightsOnScreen[i].getgroep() == 'B')
                            {
                                TrafficlightsOnScreen[i].setcolor(messagein.messageIn1.B.Cyclists[TrafficlightsOnScreen[i].getid()]);
                            }
                            if (TrafficlightsOnScreen[i].getgroep() == 'E')
                            {
                                TrafficlightsOnScreen[i].setcolor(messagein.messageIn2.E.Cyclists[TrafficlightsOnScreen[i].getid()]);
                            }
                            if (TrafficlightsOnScreen[i].getgroep() == 'F')
                            {
                                TrafficlightsOnScreen[i].setcolor(messagein.messageIn2.F.Cyclists[TrafficlightsOnScreen[i].getid()]);
                            }
                        }



                    }
                }
                
                ticker = 0;
            }
        }

        public void reconnect()
        {
            if (tcp.getconnected() == false)
            {
                connectionticker++;

                if (connectionticker == 1000)
                {
                    tcp.Connect(ipadress, 12345);
                    connectionticker = 0;
                }

            }

        }
    }
}