﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleServer
{
    class Server
    {
        private IPEndPoint localEndPoint;
        private Socket serverSocket;
        private bool isRunning = false;
        public SortedList<int, ClientConnection> connectionList;

        public Server(int port, int backlog)
        {
            //Set up server main Socket and connection list.
            localEndPoint = new IPEndPoint(IPAddress.Any, port);
            serverSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(localEndPoint);
            serverSocket.Listen(backlog);
            connectionList = new SortedList<int, ClientConnection>();
        }

        public void Start()
        {
            isRunning = true;

            while (isRunning)
            {
                //Accept incoming connection, generate random indentifier and send to client.
                Socket clientSocket = serverSocket.Accept();
                int identifier = Int32.Parse(GenerateRandomIdentifier()); 
                ClientConnection clientConnection = new ClientConnection(clientSocket, identifier, ref connectionList);
                //Check if identifier already exists.
                if (connectionList.ContainsKey(identifier))
                {
                    //generate new identifier
                    identifier = Int32.Parse(GenerateRandomIdentifier());                  
                }
                else
                {
                    connectionList.Add(identifier, clientConnection);
                }
                
            }
        }

        public void Stop()
        {
            isRunning = false;
            serverSocket.Close();
        }

        private string GenerateRandomIdentifier()
        {
            Random r = new Random();
            int randomNumber = r.Next(1000000);
            string sixDigitIdentifier = randomNumber.ToString("D5");
            return sixDigitIdentifier;
        }


    }
}
