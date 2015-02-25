using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace ConsoleServer
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            MessageService service = new MessageService();
            service.Start();
        }
    }
}
