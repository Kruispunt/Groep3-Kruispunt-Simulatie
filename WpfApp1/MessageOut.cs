using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    [DataContractAttribute]
    public class MessageOut
    {
        [DataMember]
        public string trafficlightid;

        [DataMember]
        public int detectielus;

        [DataMember]
        public int prioriteit;
    
    }
}
