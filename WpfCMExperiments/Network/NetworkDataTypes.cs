using AnimalChessLibrary;
using SocketWrapperLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfCMExperiments.Network.Gameplay;

namespace WpfCMExperiments.Network
{
    public enum NetworkDataTypes
    {
        // General (1 - 50)

        // Network connection (51 - 100)
        ConnectionNicknameSharing = 51,

        // Gameplay (100 - 130)
        GenericGameplay = 100,
        Move = 101,
        DrawRequest = 102,
        Resign = 103,
    }

    public static class NetworkDataTypeHelper
    {
        public static ISocketMessage InstantiateClassFromByteArray(byte[] data)
        {
            NetworkDataTypes netDataType = GetNetworkDataTypeFromClassId(data[0]);
            //byte[] newData = new byte[data.Length - 1];
            //Array.Copy(data, 1, newData, 0, newData.Length);

            switch (netDataType)
            {
                case NetworkDataTypes.ConnectionNicknameSharing:
                    return SocketMessage.UnformatByteArrayToClass(data);

                case NetworkDataTypes.Move:
                    return MoveSocketMessage.InstantiateFromByteArray(data);

                default:
                    throw new NotSupportedException();

            }
        }

        public static NetworkDataTypes GetNetworkDataTypeFromClassId(byte classId)
        {
            NetworkDataTypes netDataType = (NetworkDataTypes)classId; // May throw an exception if the conversion fails ig...

            return netDataType;
        }
    }
}
