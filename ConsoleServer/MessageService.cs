using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Timers;
using System.Threading.Tasks;

namespace ConsoleServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,InstanceContextMode=InstanceContextMode.PerSession)]
    public class MessageService : iMessagingService
    {
        private static ConcurrentDictionary<int, iMessagingServiceCallback> clients = new ConcurrentDictionary<int, iMessagingServiceCallback>();
        private Timer heartbeat = new Timer(5000);
        private static BlockingCollection<BroadcastMessage> msgQueue;

        public void Start()
        {
            Uri baseAddress = new Uri("net.tcp://localhost:11111/Testing");
            msgQueue = new BlockingCollection<BroadcastMessage>();
            Task.Factory.StartNew(() => broadcast());
            
            var host = new ServiceHost(typeof(MessageService));
            host.AddServiceEndpoint(typeof(iMessagingService), new NetTcpBinding(), baseAddress);
            host.Open();

            Console.WriteLine("WCF Messaging Service started at {0}", DateTime.Now.ToString());
            heartbeat.Elapsed += heartbeat_Elapsed;
            heartbeat.Enabled = true;
            Console.WriteLine("Press any key to quit");
            Console.ReadLine();
            host.Close();
        }

        void heartbeat_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                foreach (var client in clients)
                {
                    if (((ICommunicationObject)client.Value).State == CommunicationState.Opened)
                    {
                        client.Value.KeepAlive();
                    }
                    else
                    {
                        killClient(client.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("heartbeat failed, exception: {0}", ex.Message);
            }
        }

        public void Connect()
        {
            iMessagingServiceCallback client = OperationContext.Current.GetCallbackChannel<iMessagingServiceCallback>();
            int hash = client.GetHashCode();

            if (clients.TryAdd(hash, client))
            {
                Console.WriteLine("client {0} connected at {1}, total clients: {2}", hash, DateTime.Now.ToString(), clients.Count);
            }
        }

        public void Disconnect()
        {
            iMessagingServiceCallback client = OperationContext.Current.GetCallbackChannel<iMessagingServiceCallback>();
            int hash = client.GetHashCode();
            
            if (clients.TryRemove(hash, out client))
            {
                Console.WriteLine("client {0} disconnected at {1}, total clients: {2}", hash, DateTime.Now.ToString(), clients.Count);
            }
        }

        public void PublishMessage(string msg)
        {
            iMessagingServiceCallback sender = OperationContext.Current.GetCallbackChannel<iMessagingServiceCallback>();
            Task.Factory.StartNew(() =>
            {
                string theTime = DateTime.Now.ToString();
                Console.WriteLine("Received from {0} at {1}: {2}",sender.GetHashCode().ToString(), theTime, msg);
                string theMsg = string.Format("{0} | {1}", theTime, msg);
                BroadcastMessage bcast = new BroadcastMessage(sender, theMsg);

                msgQueue.Add(bcast);
            });
        }

        private void broadcast()
        {
            foreach (BroadcastMessage msg in msgQueue.GetConsumingEnumerable())
            {
                foreach (var client in clients)
                {
                    try
                    {
                        if (client.Value != msg.sender) client.Value.NewBroadcast(msg.msg);
                    }
                    catch
                    {
                        killClient(client.Value);
                    }
                }
            }
        }

        private void killClient(iMessagingServiceCallback client)
        {
            try
            {
                int hash = client.GetHashCode();
                if (clients.TryRemove(hash, out client))
                {
                    Console.WriteLine("killed client {0} at {1}", hash, DateTime.Now.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception killing client {0}: {1}", client.GetHashCode().ToString(), e.Message);
            }
        }

        private void checkHeartbeat()
        {
            foreach (var client in clients)
            {
                if (!client.Value.KeepAlive()) killClient(client.Value);
            }
        }
    }

    public class BroadcastMessage
    {
        public iMessagingServiceCallback sender { get; set; }
        public string msg { get; set; }

        public BroadcastMessage(iMessagingServiceCallback sender, string msg)
        {
            this.sender = sender;
            this.msg = msg;
        }
    }
}
