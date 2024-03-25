using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    [DataContractAttribute]
    public class MessageIn
    {
        [DataMember]
        public string trafficlightid;

        [DataMember]
        public int color;
    }
}
