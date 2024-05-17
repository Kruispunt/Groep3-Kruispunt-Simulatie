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
using System.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Tcp tcp;
        public DispatcherTimer timer = new DispatcherTimer();
        public DispatcherTimer comtimer = new DispatcherTimer();
        public Mainmessage message1 = new Mainmessage();
        public MainMessageIn messagein= new MainMessageIn();

        public List<Vehicle> vehicles = new List<Vehicle>();
        public List<TrafficLight> TrafficlightsOnScreen = new List<TrafficLight>();

        public List<Point>spawnpoints = new List<Point>(); 
        public List<Turnpoint> turnpoints = new List<Turnpoint>();
        public List<Switchlanepoint> switchlanepoints = new List<Switchlanepoint>();

        public int ticker =0;
        public int connectionticker=0;

        public int screenleft = 1600;
        public int screenbottom = 900;

        
        public int maxentities = 300;

        public int spawncars=9;
        public int spawncarsrate=500;

        public int spawnbus=1;
        public int spawnbusrate=300;

        public int spawnbicycle=9;
        public int spawnbicyclerate=200;        
        
        public int spawnpedestrian=49;
        public int spawnpedestrianrate=200;

        public float carspeed = 1f;
        public float bicyclespeed = 0.5f;
        public float walkspeed = 0.1f;


        public int port = 8080;
        public int sendtime = 500;
        public int reconnecttime = 100;
        public string ipadress = "192.168.137.1";


        public MainWindow()
        {
            InitializeComponent();

            //create a ticker on main thread due to ui
            timer.Interval =TimeSpan.FromMilliseconds(1);
            timer.Tick += Engine;
            timer.Start();

            tcp = new Tcp();
            tcp.Connect(ipadress, port);

            //create seperate thread for tcp and connections
            Thread comthread = new Thread(Comtimer);
            comthread.Start();

            Canvas.Focus();

            trafficlights();
            PointSet();

        }
        
        /// <summary>
        /// main loop on the main thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Engine(object sender, EventArgs e)
        {

            spawn();

            ticks();
            carcollision();
            PedesBikecollision();

            outabounds();

        }

        /// <summary>
        /// main tick code thats run evry milisecond
        /// </summary>
        public void ticks()
        {
            for (int i = 0; i < vehicles.Count(); i++)
            {
                vehicles[i].Tick();
                //add collision detect
                Canvas.SetTop(vehicles[i].GetRectangle(),vehicles[i].getposition().Y);
                Canvas.SetLeft(vehicles[i].GetRectangle(),vehicles[i].getposition().X);
                if (vehicles[i].GetType() == typeof(Bycicle))
                {
                    Bycicle b = (Bycicle)vehicles[i];
                    b.checkturnpoints(turnpoints);
                }
                if (vehicles[i].GetType() == typeof(Pedestrian))
                {
                    Pedestrian p = (Pedestrian)vehicles[i];
                    p.checkturnpoints(turnpoints);
                }
            }

            for (int i = 0; i < TrafficlightsOnScreen.Count(); i++)
            {
                TrafficlightsOnScreen[i].Tick();
                TrafficlightsOnScreen[i].SetLoop(vehicles);
            }
        }

        /// <summary>
        /// checks if something is of screen and then removes it.
        /// </summary>
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

        /// <summary>
        /// car collision
        /// </summary>
        public void carcollision()
        {
            List<Car> cars = vehicles.OfType<Car>().ToList();

            foreach (Car car in cars)
            {
                foreach(Car car1 in cars)
                {
                    if (car == car1)
                    {
                        break;
                    }
                    else
                    {
                        if (car.closeby(car1.getposition(), 1))
                        {
                            car.setMoving(false);
                            break;
                        }
                        car.setMoving(true);
                    }
                    
                }
            }
        }

        public void PedesBikecollision()
        {
            List<Pedestrian> Pedestrians = vehicles.OfType<Pedestrian>().ToList();
            List<Bycicle> Bikes = vehicles.OfType<Bycicle>().ToList();

            List<Vehicle> comb = [.. Bikes, .. Pedestrians];

            foreach (Vehicle pedes in comb)
            {
                foreach (Vehicle Bike in comb)
                {
                    if (pedes == Bike)
                    {
                        break;
                    }
                    if (pedes.closeby(Bike.getposition(), 1))
                    {
                        pedes.setMoving(false);
                        break;
                    }
                    pedes.setMoving(true);
                    

                }
            }
        }

    #region ----------------------------create vehicles--------------------------------

    /// <summary>
    /// spawn bus, cyclist,car, pedestrian at set intervals
    /// </summary>
    public void spawn()
        {
            if (spawnbus >= spawnbusrate && vehicles.Count() < maxentities)
            {
                CreateNewBus();
                spawnbus = 0;
            }
            else { spawnbus++; }

            if (spawncars >= spawncarsrate && vehicles.Count() < maxentities)
            {
                CreateNewCar();
                spawncars = 0;
            }
            else{ spawncars++; }

            if (spawnbicycle >= spawnbicyclerate && vehicles.Count() < maxentities)
            {
                CreateNewBicycle();
                spawnbicycle = 0;
            }
            else { spawnbicycle++; }

            if (spawnpedestrian >= spawnpedestrianrate && vehicles.Count() < maxentities)
            {
                CreateNewPedes();
                spawnpedestrian = 0;
            }
            else { spawnpedestrian++; }
        }

        /// <summary>
        /// create new car
        /// </summary>
        public void CreateNewCar()
        {
            Random r = new Random();
            int i = r.Next(spawnpoints.Count());
            Point spawnpoint = spawnpoints[i];

            while(spawnpoint.getspawnpointtype() != Spawnpointtype.Car)
            {
                i = r.Next(spawnpoints.Count());
                spawnpoint = spawnpoints[i];
            }

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

        /// <summary>
        /// create a bus
        /// </summary>
        public void CreateNewBus()
        {

            Random r = new Random();
            int i = r.Next(spawnpoints.Count());
            Point spawnpoint = spawnpoints[i];

            while (spawnpoint.getspawnpointtype() != Spawnpointtype.Bus)
            {
                i = r.Next(spawnpoints.Count());
                spawnpoint = spawnpoints[i];
            }

            i = r.Next(10);

            if (spawnpoint.getpoint()==new Vector2(184, 900))
            {
                i = 2;
            }

            Rectangle newBus = new Rectangle
            {
                Tag = "Bus",
                Height = 15,
                Width = 10,
                Fill = Brushes.CornflowerBlue
            };

            Canvas.SetTop(newBus, spawnpoint.getpoint().Y);
            Canvas.SetLeft(newBus, spawnpoint.getpoint().X);
            Canvas.Children.Add(newBus);

            vehicles.Add(new Bus(spawnpoint.getpoint().X, spawnpoint.getpoint().Y, newBus, spawnpoint.GetDrivedirection(), 10, 15, carspeed,i));
        }

        /// <summary>
        /// create a new bicycle
        /// </summary>
        public void CreateNewBicycle()
        {
            Random r = new Random();
            int i = r.Next(spawnpoints.Count());
            Point spawnpoint = spawnpoints[i];

            while (spawnpoint.getspawnpointtype() != Spawnpointtype.Bicycle)
            {
                i = r.Next(spawnpoints.Count());
                spawnpoint = spawnpoints[i];
            }

            Rectangle newBicycle = new Rectangle
            {
                Tag = "Bicycle",
                Height = 4,
                Width = 4,
                Fill = Brushes.Purple
            };

            Canvas.SetTop(newBicycle, spawnpoint.getpoint().Y);
            Canvas.SetLeft(newBicycle, spawnpoint.getpoint().X);
            Canvas.Children.Add(newBicycle);

            vehicles.Add(new Bycicle(spawnpoint.getpoint().X, spawnpoint.getpoint().Y, newBicycle, spawnpoint.GetDrivedirection(), 4, 4, bicyclespeed));
        }


        /// <summary>
        /// create a new bicycle
        /// </summary>
        public void CreateNewPedes()
        {
            Random r = new Random();
            int i = r.Next(spawnpoints.Count());
            Point spawnpoint = spawnpoints[i];

            while (spawnpoint.getspawnpointtype() != Spawnpointtype.Pedestrian)
            {
                i = r.Next(spawnpoints.Count());
                spawnpoint = spawnpoints[i];
            }

            Rectangle newPedes = new Rectangle
            {
                Tag = "Pedestrian",
                Height = 3,
                Width = 3,
                Fill = Brushes.LimeGreen
            };

            Canvas.SetTop(newPedes, spawnpoint.getpoint().Y);
            Canvas.SetLeft(newPedes, spawnpoint.getpoint().X);
            Canvas.Children.Add(newPedes);

            vehicles.Add(new Pedestrian(spawnpoint.getpoint().X, spawnpoint.getpoint().Y, newPedes, spawnpoint.GetDrivedirection(), 3, 3, walkspeed));
        }

        #endregion

        //---------------------------messages and connection-------------------------
        /// <summary>
        /// code for setting up the communication timer on a seperate thread
        /// </summary>
        public void Comtimer()
        {
            comtimer.Interval = TimeSpan.FromMilliseconds(1);
            comtimer.Tick += communicate;
            comtimer.Start();
        }

        /// <summary>
        /// communication timer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void communicate(object sender, EventArgs e)
        {
            reconnect();
            updateMessage();
            messagesendandreceive();
        }

        /// <summary>
        /// update send message
        /// </summary>
        public void updateMessage()
        {
            message1 = new Mainmessage();
            for (int i = 0; i < TrafficlightsOnScreen.Count(); i++)
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

                if (TrafficlightsOnScreen[i].GettrafficType() == typeTrafficlight.PedestrianLight)
                {
                    switch (TrafficlightsOnScreen[i].getgroep())
                    {
                        case 'A':
                            message1.messageOut1.A.Pedestrians[TrafficlightsOnScreen[i].getid()] = (pedesRoadInfo)TrafficlightsOnScreen[i].GetRoadInfo();
                            break;
                        case 'B':
                            message1.messageOut1.B.Pedestrians[TrafficlightsOnScreen[i].getid()] = (pedesRoadInfo)TrafficlightsOnScreen[i].GetRoadInfo();
                            break;
                        case 'E':
                            message1.messageOut2.E.Pedestrians[TrafficlightsOnScreen[i].getid()] = (pedesRoadInfo)TrafficlightsOnScreen[i].GetRoadInfo();
                            break;
                        case 'F':
                            message1.messageOut2.F.Pedestrians[TrafficlightsOnScreen[i].getid()] = (pedesRoadInfo)TrafficlightsOnScreen[i].GetRoadInfo();
                            break;
                    }
                }

                if (TrafficlightsOnScreen[i].GettrafficType() == typeTrafficlight.Buslight)
                {
                    BusTrafficLight buslight = (BusTrafficLight)TrafficlightsOnScreen[i];
                    if (buslight.GetBus() != null)
                    {
                        if(buslight.getgroep()== 'B'){
                            message1.messageOut1.B.busses.Add(buslight.getbusline());
                            break;
                        }
                        else if (buslight.getgroep() == 'E')
                        {
                            message1.messageOut2.E.busses.Add(buslight.getbusline());
                            break;
                        }
                      
                    }
                }
            }
        }

        /// <summary>
        /// message send and receive
        /// </summary>
        public void messagesendandreceive()
        {
            ticker++;
            if (tcp.getconnected()==true)
            {

                if(ticker == sendtime)
                {
                    if (message1 == null)
                    {
                        return;
                    }

                    test.Text = tcp.sendmessages(message1);


                    ticker = 0;
                }

                messagein = tcp.receivemessages();

                if (messagein.messageIn1.C.Cars.Count !=0 && messagein.messageIn2.D.Cars.Count() != 0)
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
                        if (TrafficlightsOnScreen[i].GettrafficType() == typeTrafficlight.PedestrianLight)
                        {
                            if (TrafficlightsOnScreen[i].getgroep() == 'A')
                            {
                                TrafficlightsOnScreen[i].setcolor(messagein.messageIn1.A.Pedestrians[TrafficlightsOnScreen[i].getid()]);
                            }
                            if (TrafficlightsOnScreen[i].getgroep() == 'B')
                            {
                                TrafficlightsOnScreen[i].setcolor(messagein.messageIn1.B.Pedestrians[TrafficlightsOnScreen[i].getid()]);
                            }
                            if (TrafficlightsOnScreen[i].getgroep() == 'E')
                            {
                                TrafficlightsOnScreen[i].setcolor(messagein.messageIn2.E.Pedestrians[TrafficlightsOnScreen[i].getid()]);
                            }
                            if (TrafficlightsOnScreen[i].getgroep() == 'F')
                            {
                                TrafficlightsOnScreen[i].setcolor(messagein.messageIn2.F.Pedestrians[TrafficlightsOnScreen[i].getid()]);
                            }
                        }
                        if (TrafficlightsOnScreen[i].GettrafficType() == typeTrafficlight.Buslight)
                        {
                            if (TrafficlightsOnScreen[i].getgroep() == 'B')
                            {
                                TrafficlightsOnScreen[i].setcolor(messagein.messageIn1.B.Busses[TrafficlightsOnScreen[i].getid()]);
                            }
                            if (TrafficlightsOnScreen[i].getgroep() == 'E')
                            {
                                TrafficlightsOnScreen[i].setcolor(messagein.messageIn2.E.Busses[TrafficlightsOnScreen[i].getid()]);
                            }

                        }


                    }
                }

            }
        }

        /// <summary>
        /// reconnect with server when no connection has been found
        /// </summary>
        public void reconnect()
        {
            if (tcp.getconnected() == false)
            {
                connectionticker++;

                if (connectionticker == reconnecttime)
                {
                    tcp.Connect(ipadress, port);
                    connectionticker = 0;
                }

            }

        }

        //--------------------------------initialisation-----------------------------

        /// <summary>
        /// all the code for spawnpoint and turnpoint initialisation
        /// </summary>
        public void PointSet()
        {
            #region -----------------------SpawnPoints------------------------------------
            //a points
            spawnpoints.Add(new Point(new Vector2(0, 400), Drivedirection.East, Spawnpointtype.Car));
            spawnpoints.Add(new Point(new Vector2(0, 411), Drivedirection.East, Spawnpointtype.Car));
            spawnpoints.Add(new Point(new Vector2(0, 422), Drivedirection.East, Spawnpointtype.Car));
            spawnpoints.Add(new Point(new Vector2(0, 433), Drivedirection.East, Spawnpointtype.Car));

            //b points          
            spawnpoints.Add(new Point(new Vector2(140, 900), Drivedirection.North, Spawnpointtype.Car));
            spawnpoints.Add(new Point(new Vector2(151, 900), Drivedirection.North, Spawnpointtype.Car));
            spawnpoints.Add(new Point(new Vector2(162, 900), Drivedirection.North, Spawnpointtype.Car));
            spawnpoints.Add(new Point(new Vector2(173, 900), Drivedirection.North, Spawnpointtype.Car));

            //F points          
            spawnpoints.Add(new Point(new Vector2(1600, 381), Drivedirection.West, Spawnpointtype.Car));
            spawnpoints.Add(new Point(new Vector2(1600, 370), Drivedirection.West, Spawnpointtype.Car));
            spawnpoints.Add(new Point(new Vector2(1600, 359), Drivedirection.West, Spawnpointtype.Car));
            spawnpoints.Add(new Point(new Vector2(1600, 348), Drivedirection.West, Spawnpointtype.Car));

            //e points         
            spawnpoints.Add(new Point(new Vector2(922, 0), Drivedirection.South, Spawnpointtype.Car));
            spawnpoints.Add(new Point(new Vector2(911, 0), Drivedirection.South, Spawnpointtype.Car));
            spawnpoints.Add(new Point(new Vector2(900, 0), Drivedirection.South, Spawnpointtype.Car));

            //bus points
            spawnpoints.Add(new Point(new Vector2(184, 900), Drivedirection.North, Spawnpointtype.Bus));
            spawnpoints.Add(new Point(new Vector2(933, 0), Drivedirection.South, Spawnpointtype.Bus));

            //bycicle
            spawnpoints.Add(new Point(new Vector2(0, 334), Drivedirection.East, Spawnpointtype.Bicycle));
            spawnpoints.Add(new Point(new Vector2(1600, 330), Drivedirection.West, Spawnpointtype.Bicycle));
            spawnpoints.Add(new Point(new Vector2(1600, 450), Drivedirection.West, Spawnpointtype.Bicycle));
            spawnpoints.Add(new Point(new Vector2(0, 454), Drivedirection.East, Spawnpointtype.Bicycle));
            spawnpoints.Add(new Point(new Vector2(89, 900), Drivedirection.North, Spawnpointtype.Bicycle));
            spawnpoints.Add(new Point(new Vector2(989, 900), Drivedirection.North, Spawnpointtype.Bicycle));

            //pedestrian
            spawnpoints.Add(new Point(new Vector2(0, 325), Drivedirection.East,    Spawnpointtype.Pedestrian));
            spawnpoints.Add(new Point(new Vector2(1600, 322), Drivedirection.West, Spawnpointtype.Pedestrian));
            spawnpoints.Add(new Point(new Vector2(1600, 460), Drivedirection.West, Spawnpointtype.Pedestrian));
            spawnpoints.Add(new Point(new Vector2(0, 463), Drivedirection.East,    Spawnpointtype.Pedestrian));
            spawnpoints.Add(new Point(new Vector2(79, 900), Drivedirection.North,  Spawnpointtype.Pedestrian));
            spawnpoints.Add(new Point(new Vector2(999, 900), Drivedirection.North, Spawnpointtype.Pedestrian));
            #endregion

            #region --------------------------turnpoints bicycles---------------------------------

            turnpoints.Add(new Turnpoint(new Vector2(85, 334), Drivedirection.South, false, Spawnpointtype.Bicycle,Drivedirection.East,5));
            turnpoints.Add(new Turnpoint(new Vector2(85, 330), Drivedirection.South, false, Spawnpointtype.Bicycle,Drivedirection.West,5));

            turnpoints.Add(new Turnpoint(new Vector2(85, 454), Drivedirection.South, false, Spawnpointtype.Bicycle, Drivedirection.East, 5));
            turnpoints.Add(new Turnpoint(new Vector2(85, 450), Drivedirection.South, false, Spawnpointtype.Bicycle, Drivedirection.West, 5));

            turnpoints.Add(new Turnpoint(new Vector2(89, 454), Drivedirection.North, false, Spawnpointtype.Bicycle, Drivedirection.East, 5));
            turnpoints.Add(new Turnpoint(new Vector2(89, 450), Drivedirection.North, false, Spawnpointtype.Bicycle, Drivedirection.West, 5));


            turnpoints.Add(new Turnpoint(new Vector2(985, 334), Drivedirection.South, false, Spawnpointtype.Bicycle, Drivedirection.East, 5));
            turnpoints.Add(new Turnpoint(new Vector2(985, 330), Drivedirection.South, false, Spawnpointtype.Bicycle, Drivedirection.West, 5));

            turnpoints.Add(new Turnpoint(new Vector2(985, 454), Drivedirection.South, false, Spawnpointtype.Bicycle, Drivedirection.East, 5));
            turnpoints.Add(new Turnpoint(new Vector2(985, 450), Drivedirection.South, false, Spawnpointtype.Bicycle, Drivedirection.West, 5));

            turnpoints.Add(new Turnpoint(new Vector2(989, 454), Drivedirection.North, false, Spawnpointtype.Bicycle, Drivedirection.East, 5));
            turnpoints.Add(new Turnpoint(new Vector2(989, 450), Drivedirection.North, false, Spawnpointtype.Bicycle, Drivedirection.West, 5));

            turnpoints.Add(new Turnpoint(new Vector2(89, 334), Drivedirection.East, false, Spawnpointtype.Bicycle, Drivedirection.North, 5));
            turnpoints.Add(new Turnpoint(new Vector2(89, 330), Drivedirection.West, true, Spawnpointtype.Bicycle, Drivedirection.North,  5));

            turnpoints.Add(new Turnpoint(new Vector2(989, 334), Drivedirection.East, false, Spawnpointtype.Bicycle, Drivedirection.North, 5));
            turnpoints.Add(new Turnpoint(new Vector2(989, 330), Drivedirection.West, true, Spawnpointtype.Bicycle, Drivedirection.North,  5));

            turnpoints.Add(new Turnpoint(new Vector2(89, 454), Drivedirection.East, false, Spawnpointtype.Bicycle, Drivedirection.North, 5));
            turnpoints.Add(new Turnpoint(new Vector2(89, 450), Drivedirection.West, false, Spawnpointtype.Bicycle, Drivedirection.North, 5));


            turnpoints.Add(new Turnpoint(new Vector2(989, 454), Drivedirection.East, false, Spawnpointtype.Bicycle, Drivedirection.North, 5));
            turnpoints.Add(new Turnpoint(new Vector2(989, 450), Drivedirection.West, false, Spawnpointtype.Bicycle, Drivedirection.North, 5));
            #endregion

            #region --------------------------turnpoints pedestrian---------------------------------

            turnpoints.Add(new Turnpoint(new Vector2(76, 325),   Drivedirection.South, false, Spawnpointtype.Pedestrian, Drivedirection.East,  5));
            turnpoints.Add(new Turnpoint(new Vector2(76, 322),   Drivedirection.South, false, Spawnpointtype.Pedestrian, Drivedirection.West,  5));
                                                                                                                                               
            turnpoints.Add(new Turnpoint(new Vector2(76, 463),   Drivedirection.South, false, Spawnpointtype.Pedestrian, Drivedirection.East,  5));
            turnpoints.Add(new Turnpoint(new Vector2(76, 460),   Drivedirection.South, false, Spawnpointtype.Pedestrian, Drivedirection.West,  5));
                                                                                                                                               
            turnpoints.Add(new Turnpoint(new Vector2(79, 463),   Drivedirection.North, false, Spawnpointtype.Pedestrian, Drivedirection.East,  5));
            turnpoints.Add(new Turnpoint(new Vector2(79, 460),   Drivedirection.North, false, Spawnpointtype.Pedestrian, Drivedirection.West,  5));
                                                                                                                                               
                                                                                                                                               
            turnpoints.Add(new Turnpoint(new Vector2(996, 325),  Drivedirection.South, false, Spawnpointtype.Pedestrian, Drivedirection.East,  5));
            turnpoints.Add(new Turnpoint(new Vector2(996, 322),  Drivedirection.South, false, Spawnpointtype.Pedestrian, Drivedirection.West,  5));
                                                                                                                                               
            turnpoints.Add(new Turnpoint(new Vector2(996, 463),  Drivedirection.South, false, Spawnpointtype.Pedestrian, Drivedirection.East,  5));
            turnpoints.Add(new Turnpoint(new Vector2(996, 460),  Drivedirection.South, false, Spawnpointtype.Pedestrian, Drivedirection.West,  5));
                                                                                                                                               
            turnpoints.Add(new Turnpoint(new Vector2(999, 463), Drivedirection.North, false, Spawnpointtype.Pedestrian, Drivedirection.East,  5));
            turnpoints.Add(new Turnpoint(new Vector2(999, 460), Drivedirection.North, false, Spawnpointtype.Pedestrian, Drivedirection.West,  5));


            turnpoints.Add(new Turnpoint(new Vector2(79, 325),   Drivedirection.East,  false, Spawnpointtype.Pedestrian, Drivedirection.North, 5));
            turnpoints.Add(new Turnpoint(new Vector2(79, 322),   Drivedirection.West,  true,  Spawnpointtype.Pedestrian, Drivedirection.North, 5));
                                                                                              
            turnpoints.Add(new Turnpoint(new Vector2(999, 325), Drivedirection.East,  false, Spawnpointtype.Pedestrian, Drivedirection.North, 5));
            turnpoints.Add(new Turnpoint(new Vector2(999, 322), Drivedirection.West,  true,  Spawnpointtype.Pedestrian, Drivedirection.North, 5));
                                                                                              
            turnpoints.Add(new Turnpoint(new Vector2(79, 463),   Drivedirection.East,  false, Spawnpointtype.Pedestrian, Drivedirection.North, 5));
            turnpoints.Add(new Turnpoint(new Vector2(79, 460),   Drivedirection.West,  false, Spawnpointtype.Pedestrian, Drivedirection.North, 5));
                                                                                              
                                                                                              
            turnpoints.Add(new Turnpoint(new Vector2(999, 463), Drivedirection.East,  false, Spawnpointtype.Pedestrian, Drivedirection.North, 5));
            turnpoints.Add(new Turnpoint(new Vector2(999, 460), Drivedirection.West,  false, Spawnpointtype.Pedestrian, Drivedirection.North, 5));
            #endregion

        }

        /// <summary>
        /// all the code for trafficlight initialisation
        /// </summary>
        public void trafficlights()
        {
            #region -------------------------------------------------------------------------------Car------------------------------------------------------------------------------------------
            TrafficlightsOnScreen.Add(new CarTrafficLight(0, 'A', new Vector2(50, 400), new Vector2(25, 400), Direction.straight));//A0
            TrafficlightsOnScreen.Add(new CarTrafficLight(1, 'A', new Vector2(50, 411), new Vector2(25, 411), Direction.straight));//A1
            TrafficlightsOnScreen.Add(new CarTrafficLight(2, 'A', new Vector2(50, 422), new Vector2(25, 422), Direction.right, new Vector2(111, 422), Drivedirection.South));//A2
            TrafficlightsOnScreen.Add(new CarTrafficLight(3, 'A', new Vector2(50, 433), new Vector2(25, 433), Direction.right, new Vector2(100, 433), Drivedirection.South));//A3

            TrafficlightsOnScreen.Add(new CarTrafficLight(0, 'B', new Vector2(140, 480), new Vector2(140, 505), Direction.left, new Vector2(140, 359), Drivedirection.West));//B0 
            TrafficlightsOnScreen.Add(new CarTrafficLight(1, 'B', new Vector2(151, 480), new Vector2(151, 505), Direction.left, new Vector2(151, 348), Drivedirection.West));//B1 
            TrafficlightsOnScreen.Add(new CarTrafficLight(2, 'B', new Vector2(162, 480), new Vector2(162, 505), Direction.right, new Vector2(162, 400), Drivedirection.East));//B2 
            TrafficlightsOnScreen.Add(new CarTrafficLight(3, 'B', new Vector2(173, 480), new Vector2(173, 505), Direction.right, new Vector2(173, 411), Drivedirection.East));//B3 

            TrafficlightsOnScreen.Add(new CarTrafficLight(0, 'C', new Vector2(205, 381), new Vector2(230, 381), Direction.left, new Vector2(111, 381), Drivedirection.South));//C0
            TrafficlightsOnScreen.Add(new CarTrafficLight(1, 'C', new Vector2(205, 370), new Vector2(230, 370), Direction.left, new Vector2(100, 370), Drivedirection.South));//C1
            TrafficlightsOnScreen.Add(new CarTrafficLight(2, 'C', new Vector2(205, 359), new Vector2(230, 359), Direction.straight)); //C2 
            TrafficlightsOnScreen.Add(new CarTrafficLight(3, 'C', new Vector2(205, 348), new Vector2(230, 348), Direction.straight)); //C3 


            TrafficlightsOnScreen.Add(new CarTrafficLight(0, 'D', new Vector2(890, 400), new Vector2(865, 400), Direction.left, new Vector2(955, 400), Drivedirection.North));//D0
            TrafficlightsOnScreen.Add(new CarTrafficLight(1, 'D', new Vector2(890, 411), new Vector2(865, 411), Direction.left, new Vector2(966, 411), Drivedirection.North));//D1
            TrafficlightsOnScreen.Add(new CarTrafficLight(2, 'D', new Vector2(890, 422), new Vector2(865, 422), Direction.straight));//D2
            TrafficlightsOnScreen.Add(new CarTrafficLight(3, 'D', new Vector2(890, 433), new Vector2(865, 433), Direction.straight));//D3

            TrafficlightsOnScreen.Add(new CarTrafficLight(0, 'E', new Vector2(922, 290), new Vector2(922, 265), Direction.left, new Vector2(922, 433), Drivedirection.East));//E1 
            TrafficlightsOnScreen.Add(new CarTrafficLight(1, 'E', new Vector2(911, 290), new Vector2(911, 265), Direction.right, new Vector2(911, 359), Drivedirection.West));//E2
            TrafficlightsOnScreen.Add(new CarTrafficLight(2, 'E', new Vector2(900, 290), new Vector2(900, 265), Direction.right, new Vector2(900, 348), Drivedirection.West));//E3

            TrafficlightsOnScreen.Add(new CarTrafficLight(0, 'F', new Vector2(1010, 381), new Vector2(1035, 381), Direction.straight));//F0
            TrafficlightsOnScreen.Add(new CarTrafficLight(1, 'F', new Vector2(1010, 370), new Vector2(1035, 370), Direction.straight));//F1
            TrafficlightsOnScreen.Add(new CarTrafficLight(2, 'F', new Vector2(1010, 359), new Vector2(1035, 359), Direction.right, new Vector2(966, 359), Drivedirection.North));//F2
            TrafficlightsOnScreen.Add(new CarTrafficLight(3, 'F', new Vector2(1010, 348), new Vector2(1035, 348), Direction.right, new Vector2(955, 348), Drivedirection.North));//F3
            #endregion
            #region -------------------------------------------------------------------------------BUS-------------------------------------------------------------------------------------------
            BusTrafficLight E0 = new BusTrafficLight(0, 'E', Direction.left, new Vector2(933, 290), new Vector2(933, 422), Drivedirection.East);
            BusTrafficLight E1 = new BusTrafficLight(1, 'E', Direction.right, new Vector2(933, 290), new Vector2(933, 359), Drivedirection.West);

            E0.setneighbour(E1);
            E1.setneighbour(E0);

            TrafficlightsOnScreen.Add(E0);//E1 
            TrafficlightsOnScreen.Add(E1);//E2

            BusTrafficLight B0 = new BusTrafficLight(0, 'B', Direction.left, new Vector2(184, 480), new Vector2(184, 422), Drivedirection.East);

            TrafficlightsOnScreen.Add(B0);//E1 
            #endregion
            #region -------------------------------------------------------------------------------bicycle---------------------------------------------------------------------------------------
            TrafficlightsOnScreen.Add(new BicycleTrafficLight(0, 'A', new Vector2(85, 336)));
            TrafficlightsOnScreen.Add(new BicycleTrafficLight(1, 'A', new Vector2(89, 460)));

            TrafficlightsOnScreen.Add(new BicycleTrafficLight(0, 'B', new Vector2(80, 454))); 
            TrafficlightsOnScreen.Add(new BicycleTrafficLight(1, 'B', new Vector2(198, 450)));

            TrafficlightsOnScreen.Add(new BicycleTrafficLight(0, 'E', new Vector2(981, 330)));
            TrafficlightsOnScreen.Add(new BicycleTrafficLight(1, 'E', new Vector2(890, 334)));

            TrafficlightsOnScreen.Add(new BicycleTrafficLight(0, 'F', new Vector2(985, 340)));
            TrafficlightsOnScreen.Add(new BicycleTrafficLight(1, 'F', new Vector2(989, 447)));

            #endregion
            #region -------------------------------------------------------------------------------pedestrian------------------------------------------------------------------------------------

            TrafficlightsOnScreen.Add(new PedestrianTrafficlight(0, 'A', new Vector2(76, 344)));//A0 
            TrafficlightsOnScreen.Add(new PedestrianTrafficlight(1, 'A', new Vector2(79, 370)));//A1 
            TrafficlightsOnScreen.Add(new PedestrianTrafficlight(2, 'A', new Vector2(76, 396)));//A1 
            TrafficlightsOnScreen.Add(new PedestrianTrafficlight(3, 'A', new Vector2(79, 444)));//A1 
                                          
            TrafficlightsOnScreen.Add(new PedestrianTrafficlight(0, 'B', new Vector2(95 , 464)));//A1 
            TrafficlightsOnScreen.Add(new PedestrianTrafficlight(1, 'B', new Vector2(123, 460)));//A1 
            TrafficlightsOnScreen.Add(new PedestrianTrafficlight(2, 'B', new Vector2(136, 464)));//A1 
            TrafficlightsOnScreen.Add(new PedestrianTrafficlight(3, 'B', new Vector2(196, 460)));//A1 
                                          
            TrafficlightsOnScreen.Add(new PedestrianTrafficlight(0, 'E', new Vector2(999,444)));//A1 
            TrafficlightsOnScreen.Add(new PedestrianTrafficlight(1, 'E', new Vector2(996, 418)));//A1 
            TrafficlightsOnScreen.Add(new PedestrianTrafficlight(2, 'E', new Vector2(999,392)));//A1 
            TrafficlightsOnScreen.Add(new PedestrianTrafficlight(3, 'E', new Vector2(996, 344)));//A1 
                                          
            TrafficlightsOnScreen.Add(new PedestrianTrafficlight(0, 'F', new Vector2(979, 322)));//A1 
            TrafficlightsOnScreen.Add(new PedestrianTrafficlight(1, 'F', new Vector2(952, 326)));//A1 
            TrafficlightsOnScreen.Add(new PedestrianTrafficlight(2, 'F', new Vector2(943, 322)));//A1 
            TrafficlightsOnScreen.Add(new PedestrianTrafficlight(3, 'F', new Vector2(896, 326)));//A1 

            #endregion
            //set rectangles to trafficlights
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

    }
}