﻿using System;
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
        public BlockIn A;
        //public BlockIn B;
        //public BlockIn C;
    }

    [DataContractAttribute]
    public class BlockIn
    {
        [DataMember]
        public List<int> Cars;
        //list pedestrians
        //list cyclist
        //busses.
    }

}
