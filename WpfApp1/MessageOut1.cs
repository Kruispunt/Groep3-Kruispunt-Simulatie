using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.DataContracts;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    [DataContract]
    public class Mainmessage
    {
        [DataMember(Name = "1")]
        public MessageOut1 messageOut1;

        [DataMember(Name = "2")]
        public MessageOut2 messageOut2;

        public Mainmessage() { 
            messageOut1 = new MessageOut1(); 
            messageOut2 = new MessageOut2(); 
        }
    }

    [DataContractAttribute]
    public class MessageOut2
    {
        [DataMember]
        public BlockOutCarOnly D;
        [DataMember]
        public BlockOutBus E;
        [DataMember]
        public BlockOut F;

        public MessageOut2()
        {
            D = new BlockOutCarOnly(4);
            E = new BlockOutBus(3,4,2,3);
            F = new BlockOut(4,4,2);
        }
    }


    [DataContractAttribute]
    public class MessageOut1
    {
        [DataMember]
        public BlockOut A;
        [DataMember]
        public BlockOutBus B;
        [DataMember]
        public BlockOutCarOnly C;

        public MessageOut1() { 
            A = new BlockOut(4,4,2);
            B = new BlockOutBus(4,4,2,3);
            C = new BlockOutCarOnly(4);
        }
    }

    #region blocks

    [DataContractAttribute]
    public class BlockOut
    {
        [DataMember]
        public List<CarRoadInfo> Cars;
        [DataMember]
        public List<CyclistRoadInfo> Cyclists;
        [DataMember]
        public List<pedesRoadInfo> Pedestrians;


        public BlockOut(int cars, int pedes, int cycl)
        {
            Cars = new List<CarRoadInfo>();
            Pedestrians = new List<pedesRoadInfo>();
            Cyclists = new List<CyclistRoadInfo>();
            for (int i = 0; i < cars; i++)
            {
                Cars.Add(new CarRoadInfo(false,false,false));
            }
            for (int i = 0; i < pedes; i++)
            {
                Pedestrians.Add(new pedesRoadInfo(false));
            }
            for (int i = 0; i < cycl; i++)
            {
                Cyclists.Add(new CyclistRoadInfo(false));
            }

        }
    }

    [DataContractAttribute]
    public class BlockOutBus
    {
        [DataMember]
        public List<CarRoadInfo> Cars;
        [DataMember]
        public List<CyclistRoadInfo> Cyclists;
        [DataMember]
        public List<pedesRoadInfo> Pedestrians;
        [DataMember]
        public List<int> busses;

        public BlockOutBus(int cars,int pedes,int cycl,int bus)
        {
            Cars = new List<CarRoadInfo>();
            Pedestrians = new List<pedesRoadInfo>();
            Cyclists = new List<CyclistRoadInfo>();
            busses = new List<int>();

            for (int i = 0; i < cars; i++)
            {
                Cars.Add(new CarRoadInfo(false, false, false));
            }
            for (int i = 0; i < pedes; i++)
            {
                Pedestrians.Add(new pedesRoadInfo(false));
            }
            for (int i = 0; i < cycl; i++)
            {
                Cyclists.Add(new CyclistRoadInfo(false));
            }
            for (int i = 0; i < bus; i++)
            {
                busses.Add(0);
            }

        }
    }

    [DataContractAttribute]
    public class BlockOutCarOnly
    {
        [DataMember]
        public List<CarRoadInfo> Cars;

        public BlockOutCarOnly(int cars)
        {
            Cars = new List<CarRoadInfo>();
            for (int i = 0; i < cars; i++)
            {
                Cars.Add(new CarRoadInfo(false,false,false));
            }
        }
    }

    #endregion

    #region roadinfo

    [DataContractAttribute]
    public class CarRoadInfo : Info
    {
        [DataMember]
        public bool DetectNear;
        [DataMember]
        public bool DetectFar;
        [DataMember]
        public bool PrioCar;

        public CarRoadInfo(bool detectNear, bool detectFar, bool prioCar) :base()
        {
            DetectNear = detectNear;
            DetectFar = detectFar;
            PrioCar = prioCar;
        }   
    }

    [DataContractAttribute]
    public class pedesRoadInfo : Info
    {
        [DataMember]
        public bool DetectPedestrians;

        public pedesRoadInfo(bool detectPedestrians):base()
        {
            DetectPedestrians = detectPedestrians;
        }
    }

    [DataContractAttribute]
    public class CyclistRoadInfo: Info
    {
        [DataMember]
        public bool DetectCyclist;

        public CyclistRoadInfo(bool detectCyclist):base()
        {
            DetectCyclist = detectCyclist;
        }
    }

    [DataContractAttribute]
    public class Info
    {
        public Info()
        {

        }
    }
    #endregion 
}
