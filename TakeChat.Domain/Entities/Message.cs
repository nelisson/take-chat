using System;

namespace TakeChat.Domain.Entities
{
    public class Message
    {
        public string From { get; }
        public string To { get; }
        public string Channel { get; }
        public string Body { get; }

        public Message(string from, string to, string channel, string body)
        {
            if (string.IsNullOrEmpty(from.Trim()))
            {
                throw new ArgumentException("From field cannot be empty", nameof(from));
            }

            if (string.IsNullOrEmpty(to.Trim()) && string.IsNullOrEmpty(channel.Trim()))
            {
                throw new ArgumentException("Almost one field needs to be filled (To or Channel)");
            }

            if (string.IsNullOrEmpty(body.Trim()))
            {
                throw new ArgumentException("Message Body cannot be empty", nameof(body));
            }

            if (body.Trim().Length > 144)
            {
                throw new ArgumentOutOfRangeException(nameof(body), "Message Body cannot contain more than 144 characters.");
            }

            From = from;
            To = to;
            Channel = channel;
            Body = body;
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return $"{From}{To}{Channel}{Body}".GetHashCode();
        }
    }
}
