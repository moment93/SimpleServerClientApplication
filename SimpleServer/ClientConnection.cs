using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SimpleServer
{

    class ClientConnection
    {
        private Socket clientSocket;
        private Thread readThread;
        private Thread writeThread;
        private bool connected = true;
        private NetworkStream ioConnectionStream;
        private StreamReader streamReader;
        private StreamWriter streamWriter;
        private SortedList<int, ClientConnection> connectionList = new SortedList<int, ClientConnection>();
        private int connectionIdentifier;

        private ConcurrentQueue<Message> inMessages;

        public ClientConnection()
        {
        }

        public ClientConnection(Socket clientSocket, int identifier, ref SortedList<int, ClientConnection> connectionList)
        {
            this.clientSocket = clientSocket;
            this.connectionIdentifier = identifier;
            this.connectionList = connectionList;
            SetupConnection();


        }

        private void SetupConnection()
        {
            try
            {
                ioConnectionStream = new NetworkStream(clientSocket);
                streamReader = new StreamReader(ioConnectionStream);
                streamWriter = new StreamWriter(ioConnectionStream);
                inMessages = new ConcurrentQueue<Message>();
                readThread = new Thread(ReadStream);
                readThread.Start();
                writeThread = new Thread(WriteStream);
                writeThread.Start();

                Debug.WriteLine("Client Connected!");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.InnerException);
                streamWriter.Close();
                streamReader.Close();
            }
        }




        //This probably shouldnt be public... but it will be cuz bad design.
        public void AddMessageToQueue(Message msg)
        {
            inMessages.Enqueue(msg);
        }


        private void WriteStream()
        {
            try
            {

                Message message = new Message();

                while (clientSocket.Connected)
                {

                    if (!inMessages.IsEmpty)
                    {
                        inMessages.TryDequeue(out message);
                        string s = JsonConvert.SerializeObject(message);
                        streamWriter.WriteLine(s);
                        streamWriter.Flush();
                    }


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.InnerException);
                streamReader.Close();
                streamWriter.Close();
            }
        }

        private void ReadStream()
        {
            try
            {
                string incoming;
                Message m = new Message();


                while (clientSocket.Connected)
                {
                    incoming = streamReader.ReadLine();
                    m = JsonConvert.DeserializeObject<Message>(incoming);
                    Console.WriteLine(m.MessageType.ToString());

                    switch (m.MessageType)
                    {
                        case Message.Type.RequestId:
                            m.Content = connectionIdentifier.ToString();
                            AddMessageToQueue(m);
                            Console.WriteLine(m.Content);
                            break;
                        case Message.Type.User:
                            connectionList[m.To].inMessages.Enqueue(m);
                            break;
                        default:
                            break;

                    }


                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.InnerException);
                streamReader.Close();
                streamWriter.Close();
            }


        }
    }
}
