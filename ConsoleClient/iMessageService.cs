using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Security;
using System.ServiceModel;

namespace ConsoleClient
{
    [ServiceContract(CallbackContract = typeof(iMessagingServiceCallback))]
    public interface iMessagingService
    {
        [OperationContract(IsOneWay = true)]
        void PublishMessage(string s);

        [OperationContract(IsOneWay = true)]
        void Connect();

        [OperationContract(IsOneWay = true)]
        void Disconnect();
    }

    [ServiceContract]
    public interface iMessagingServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void NewBroadcast(string s);
        [OperationContract(IsOneWay = false)]
        bool KeepAlive();
    }
}
