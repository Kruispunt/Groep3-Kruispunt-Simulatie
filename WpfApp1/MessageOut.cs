using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.DataContracts;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    [DataContractAttribute]
    public class MessageOut
    {
        [DataMember]
        public BlockOut A;
        //[DataMember]
        //public BlockOut B;
        //[DataMember]
        //public BlockOut C;

        public MessageOut() { 
            A = new BlockOut();
        }
    }

    [DataContractAttribute]
    public class BlockOut
    {
        [DataMember]
        public List<CarRoadInfo> Cars;
        //list pedestrians
        //list cyclist
        //busses.

        public BlockOut()
        {
            Cars.Add(new CarRoadInfo());
            Cars.Add(new CarRoadInfo());
            Cars.Add(new CarRoadInfo());
            Cars.Add(new CarRoadInfo());
        }
    }

    [DataContractAttribute]
    public class CarRoadInfo
    {
        public bool DetectNear;
        public bool DetectFar;
        public bool PrioCar;
    }
}
