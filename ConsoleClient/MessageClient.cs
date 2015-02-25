using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace ConsoleClient
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MessageClient : iMessagingServiceCallback
    {
        public delegate void NewMessageEventHandler(string msg);
        public event NewMessageEventHandler NewMessage;

        public void NewBroadcast(string msg)
        {
            if (NewMessage != null)
            {
                NewMessage(msg);
            }
        }

        public bool KeepAlive()
        {
            return true;
        }
    }
}
