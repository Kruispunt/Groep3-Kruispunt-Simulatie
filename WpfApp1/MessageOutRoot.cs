using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class MessageOutRoot
    {
        public MessageOutRoot(MessageOut messageOut) 
        {
            this.messageOut = messageOut;

        }
        public MessageOut messageOut { get; set; }

        public bool send;
    }
}
