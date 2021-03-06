﻿using System;
using System.Text;
using IFramework.Infrastructure;

namespace IFramework.Message
{
    public static class MessageFormatHelper
    {
        private static short ToShort(byte byte1, byte byte2)
        {
            return (short) ((byte2 << 8) + byte1);
        }

        private static void FromShort(short number, out byte byte1, out byte byte2)
        {
            byte2 = (byte) (number >> 8);
            byte1 = (byte) (number & 255);
        }


        public static byte[] GetMessageBytes(this object message)
        {
            return Encoding.UTF8.GetBytes(message.ToJson());
        }

        public static byte[] GetMessageBytes(this object message, short code)
        {
            var buffer = Encoding.UTF8.GetBytes(message.ToJson());
            var wrappedBuf = new byte[buffer.Length + 2];
            byte byte0, byte1;
            FromShort(code, out byte0, out byte1);
            wrappedBuf[0] = byte0;
            wrappedBuf[1] = byte1;
            Buffer.BlockCopy(buffer, 0, wrappedBuf, 2, buffer.Length);
            return wrappedBuf;
        }

        public static short GetMessageCode(this byte[] messageBody)
        {
            return ToShort(messageBody[0], messageBody[1]);
        }

        public static string GetMessage(this byte[] messageBody)
        {
            return Encoding.UTF8.GetString(messageBody);
        }

        public static T GetMessage<T>(this byte[] messageBody)
        {
            return Encoding.UTF8.GetString(messageBody).ToJsonObject<T>();
        }

        public static object GetMessage(this byte[] messageBody, Type type)
        {
            return Encoding.UTF8.GetString(messageBody).ToJsonObject(type);
        }

        public static string GetFormattedMessage(this byte[] messageBody)
        {
            var message = string.Empty;
            if (messageBody.Length > 2)
            {
                var messageBuf = new byte[messageBody.Length - 2];
                Buffer.BlockCopy(messageBody, 2, messageBuf, 0, messageBuf.Length);
                message = Encoding.UTF8.GetString(messageBuf);
            }
            return message;
        }

        public static T GetFormattedMessage<T>(this byte[] messageBody)
        {
            var message = default(T);
            message = messageBody.GetFormattedMessage().ToJsonObject<T>();
            return message;
        }
    }
}