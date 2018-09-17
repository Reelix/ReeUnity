using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Networking;

namespace ReeCode
{
    public class ReeUNetPacket : MessageBase
    {
        public int PacketType;
        public ReePacketPayload Payload = new ReePacketPayload();

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(PacketType);
            Payload.Serialize(writer);
        }

        public override void Deserialize(NetworkReader reader)
        {
            PacketType = reader.ReadInt32();
            Payload = ReePacketPayload.Deserialize(reader);
        }

        [Serializable]
        public class ReePacketPayload : object
        {
            private int DataLength;
            public List<object> PayloadData;

            public void Add(object toAdd)
            {
                if (PayloadData == null)
                {
                    PayloadData = new List<object>();
                }
                PayloadData.Add(toAdd);
            }

            public void Serialize(NetworkWriter writer)
            {
                BinaryFormatter binFormatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                binFormatter.Serialize(ms, PayloadData);
                DataLength = (int)ms.Length;
                writer.Write(DataLength);
                writer.Write(ms.ToArray(), DataLength);
            }

            public static ReePacketPayload Deserialize(NetworkReader reader)
            {
                ReePacketPayload rpp = new ReePacketPayload();
                rpp.DataLength = reader.ReadInt32();
                byte[] restOfBytes = reader.ReadBytes(rpp.DataLength);
                var mStream = new MemoryStream(restOfBytes);
                var binFormatter = new BinaryFormatter();
                rpp.PayloadData = (List<object>)binFormatter.Deserialize(mStream);
                return rpp;
            }
        }
    }
}
