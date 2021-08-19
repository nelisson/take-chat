﻿using System;

namespace TakeChat.Domain
{
    public class Message
    {
        public string From { get; private set; }
        public string To { get; private set; }
        public string Channel { get; private set; }
        public string Body { get; private set; }

        public Message(string from, string to, string channel, string body)
        {
            if(string.IsNullOrEmpty(from.Trim()))
            {
                throw new ArgumentException("Campo remetente não pode ser deixado em branco");
            }

            if (string.IsNullOrEmpty(to.Trim()) && string.IsNullOrEmpty(channel.Trim()))
            {
                throw new ArgumentException("Pelo menos um campos precisa estar preenchido (destinatário ou sala)");
            }

            if (string.IsNullOrEmpty(body.Trim()))
            {
                throw new ArgumentException("Corpo da mensagem não pode ser deixado em branco");
            }

            if (body.Trim().Length > 144)
            {
                throw new ArgumentOutOfRangeException(nameof(body), "Corpo da mensagem não pode conter mais que 144 caracteres");
            }

            From = from;
            To = to;
            Channel = channel;
            Body = body;
        }
    }
}
