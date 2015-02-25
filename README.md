# WCF-Messaging-Service
C# .NET 4.5.1 WCF Messaging Service and Client

<b>Introduction</b><br>
This project is my demonstration of a WCF server and client application using code only (without using config or wsdl file)

<b><i>ConsoleServer</i></b><br>
It is exactly that:  a C# console application that runs a simple WCF Messaging Service.<br>
* This service is hosted with net.tcp binding to port 11111 (can be changed in the Start method of the MessageService class).<br>
* Client connections are tracked using a Concurrent Dictionary<br>
* Clients are kept alive with a hearbeat to avoid timeout issues<br>
* A Blocking Collection is used to queue and broadcast messages on a dedicated thread<br>

<b><i>ConsoleClient</i></b><br>
It is exactly that:  a c# console application that acts as a client to the above WCF messaging service<br>
* The service endpoint is in the Main method of the ClientProgram class<br>
* Client messages are published to all other connected clients (the message is not broadcast back to the sender)<br>
* Quit or Exit at the command line will disconnect from the server.<br>
