using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace SimpleClient
{
    class Client
    {
        private string tempIp = "35.204.18.58";

        private Socket clientSocket;
        private static EndPoint remoEndPoint;
        private NetworkStream ioNetworkStream;
        private StreamWriter streamWriter;
        private StreamReader streamReader;
        private Thread readerThread;
        private string id = null;
        public Queue<string> messages;


        public void Connect(string ip, string port)
        {
            try
            {
                remoEndPoint = new IPEndPoint(IPAddress.Parse(ip), Int32.Parse(port));
                clientSocket = new Socket(remoEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(remoEndPoint);
                ioNetworkStream = new NetworkStream(clientSocket);
                streamWriter = new StreamWriter(ioNetworkStream);
                streamReader = new StreamReader(ioNetworkStream);
                messages = new Queue<string>();



                Message msg = new Message(Message.Type.RequestId);
                string msgString = JsonConvert.SerializeObject(msg);
                streamWriter.WriteLine(msgString);
                streamWriter.Flush();

                do
                {
                    string incoming = streamReader.ReadLine();
                    msg = JsonConvert.DeserializeObject<Message>(incoming);
                    id = msg.Content;

                    readerThread = new Thread(ReadIncoming);
                    readerThread.Start();
                    
                } while (string.IsNullOrEmpty(id));


            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                throw;
            }
            
        }

        internal void Send(Message m)
        {
            string s = JsonConvert.SerializeObject(m);
            streamWriter.WriteLine(s);
            streamWriter.Flush();
        }

        public void ReadIncoming()
        {
            //Incoming decoded string
            string incoming;
            
            while (clientSocket.Connected)
            {
                try
                {
                    incoming = streamReader.ReadLine();
                    Message m = new Message();
                    m = JsonConvert.DeserializeObject<Message>(incoming);

                    switch (m.MessageType)
                    {
                        case Message.Type.User:
                            messages.Enqueue(string.Format("From: {0} \n {1}", m.From, m.Content));
                            break;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

            }
        }

        public string GetLastMessage()
        {
            return messages.Dequeue();
            
        }

        public string Id
        {
            get { return id; }
            set { id = Id; }
        }

    }
}
