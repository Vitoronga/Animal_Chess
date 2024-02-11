using SocketWrapperLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCMExperiments.Network.Gameplay
{


    // ------------------------------------------------------------------

    //          I DON'T THINK THIS CLASS WILL BE NECESSARY...

    // ------------------------------------------------------------------


    /*
    internal class GameplayDataPacket<T> : ISocketMessage where T : struct 
    {
        private byte _classId;
        public byte ClassId
        {
            get { return _classId; }
            set { _classId = value; }
        }

        private byte[] _data;
        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public GameplayDataPacket(byte classId)
        {
            ClassId = classId;
        }

        public byte[] FormatDataAsByteArray()
        {
            throw new NotImplementedException();
        }

        public T UnformatData()
        {

        }

        public static GameplayDataPacket CreatePacket(NetworkDataTypes type)
        {
            switch (type)
            {
                case NetworkDataTypes.GenericGameplay:
                    return new GameplayDataPacket((byte)type);
                    //break;

                case NetworkDataTypes.Move:
                    return new GameplayDataPacket((byte)type);
                    //break;

                case NetworkDataTypes.DrawRequest:
                    return new GameplayDataPacket((byte)type);
                    //break;

                case NetworkDataTypes.Resign:
                    return new GameplayDataPacket((byte)type);
                    //break;

                default:
                    throw new ArgumentException("Invalid NetworkDataTypes type", "type");

            }
        }
    }

    */
}
