using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer
{
    class Message
    {
        public enum Type
        {
            RequestId,
            System,
            User
        }

        public Message()
        {
            
        }

        public Message(Type messageType , int from, int to, string content)
        {
            MessageType = messageType;
            From = from;
            To = to;
            Content = content;
        }

        public Type MessageType { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public string Content { get; set; }
    }
}
