﻿using System;
using System.Runtime.Serialization;

namespace HttpServer
{
    [Serializable]
    public class MessageNotFoundException : Exception
    {
        public MessageNotFoundException()
        {
        }

        public MessageNotFoundException(string message) : base(message)
        {
        }

        public MessageNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
