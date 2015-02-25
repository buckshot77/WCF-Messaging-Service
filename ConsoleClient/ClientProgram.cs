    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ServiceModel;

    namespace ConsoleClient
    {
        class ClientProgram
        {
            static void Main(string[] args)
            {
                var callback = new MessageClient();
                callback.NewMessage += new MessageClient.NewMessageEventHandler(showMessage);
                var instanceContext = new InstanceContext(callback);

                var myBinding = new NetTcpBinding();
                var myEndPoint = new EndpointAddress("net.tcp://localhost:11111/Testing");
                var myChannelFactory = new DuplexChannelFactory<iMessagingService>(instanceContext, myBinding, myEndPoint);
                string input = "";

                iMessagingService client = null;

                try
                {
                    client = myChannelFactory.CreateChannel();
                    client.Connect();

                    while (true)
                    {
                        Console.WriteLine("Enter message to send:");
                        input = Console.ReadLine();
                        if (input != "")
                            
                        {
                            if (input.ToUpper() == "EXIT" || input.ToUpper() == "QUIT")
                            {
                                break;
                            }
                            else
                            {
                                client.PublishMessage(input);
                            }
                        }
                    }
                    client.Disconnect();
                    ((ICommunicationObject)client).Close();
                    Console.WriteLine("client exiting...");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e.Message);
                    if (client != null)
                    {
                        client.Disconnect();
                        Console.WriteLine("aborting client");
                        ((ICommunicationObject)client).Abort();
                    }
                }
            }

            private static void showMessage(string msg)
            {
                Console.WriteLine(msg);
            }
        }
    }
