using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    [DataContractAttribute]
    public class MainMessageIn
    {
        [DataMember(Name = "1")]
        public MessageIn1 messageIn1;
        [DataMember(Name = "2")]
        public MessageIn2 messageIn2;

        public MainMessageIn()
        {
            messageIn1 = new MessageIn1();
            messageIn2 = new MessageIn2();  
        }
    }



    [DataContractAttribute]
    public class MessageIn1
    {
        [DataMember]
        public BlockinPedCyc A;
        [DataMember]
        public Blockinbus B;
        [DataMember]
        public BlockIn C;

        public MessageIn1() { 
            A = new BlockinPedCyc(); 
            B = new Blockinbus(); 
            C = new BlockIn(); 
        }
    }

    public class MessageIn2
    {
        [DataMember]
        public BlockIn D;
        [DataMember]
        public Blockinbus E;
        [DataMember]
        public BlockinPedCyc F;

        public MessageIn2()
        {
            D = new BlockIn();
            E = new Blockinbus();
            F = new BlockinPedCyc();
        }
    }


    [DataContractAttribute]
    public class BlockIn
    {
        [DataMember]
        public List<int> Cars;

        public BlockIn() {  
            Cars = new List<int>(); 
        }
    }

    public class BlockinPedCyc :BlockIn
    {
        [DataMember]
        public List<int> Cyclists;
        [DataMember]
        public List<int> Pedestrians;
        public BlockinPedCyc():base()
        {
            Cyclists = new List<int>();
            Pedestrians = new List<int>();
        }

    }
    public class Blockinbus:BlockinPedCyc
    {
        [DataMember]
        public List<int> Busses;

        public Blockinbus():base() {
            Busses = new List<int>();
        }
    }

}
