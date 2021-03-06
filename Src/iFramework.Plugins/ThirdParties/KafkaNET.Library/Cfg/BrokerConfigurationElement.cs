﻿using System.Configuration;
using System.Globalization;
using System.Net;

namespace Kafka.Client.Cfg
{
    public class BrokerConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true)]
        public int Id => (int) this["id"];

        [ConfigurationProperty("host", IsRequired = true)]
        public string Host
        {
            get => (string) this["host"];

            set => this["host"] = value;
        }

        [ConfigurationProperty("port", IsRequired = true)]
        public int Port
        {
            get => (int) this["port"];

            set => this["port"] = value;
        }

        protected override void PostDeserialize()
        {
            base.PostDeserialize();
            IPAddress ipAddress;
            if (!IPAddress.TryParse(Host, out ipAddress))
            {
                var addresses = Dns.GetHostAddresses(Host);
                if (addresses.Length > 0)
                {
                    Host = addresses[0].ToString();
                }
                else
                {
                    throw new ConfigurationErrorsException(string.Format(CultureInfo.CurrentCulture,
                                                                         "Could not resolve the address: {0}.", Host));
                }
            }
        }

        public string GetBrokerPartitionInfoAsString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}", Id, Host, Port);
        }
    }
}