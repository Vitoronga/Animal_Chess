using SocketWrapperLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCMExperiments.Network.Gameplay
{
    public class MoveSocketMessage : ISocketMessage
    {
        public byte ClassId { get; init; }
        public (byte x, byte y) OriginTile { get; init; }
        public (byte x, byte y) TargetTile { get; init; }

        public MoveSocketMessage((byte, byte) originTile, (byte, byte) targetTile) : this(originTile, targetTile, 101) { }
        private MoveSocketMessage((byte, byte) originTile, (byte, byte) targetTile, byte classId)
        {
            ClassId = classId;
            OriginTile = originTile;
            TargetTile = targetTile;
        }

        public byte[] FormatDataAsByteArray()
        {
            byte[] byteArray = new byte[9];
            byteArray[0] = ClassId;

            SocketMessageProtocol.GetFormattedValue(OriginTile.x).CopyTo(byteArray, 1);
            SocketMessageProtocol.GetFormattedValue(OriginTile.y).CopyTo(byteArray, 3);
            SocketMessageProtocol.GetFormattedValue(TargetTile.x).CopyTo(byteArray, 5);
            SocketMessageProtocol.GetFormattedValue(TargetTile.y).CopyTo(byteArray, 7);

            return byteArray;
        }

        public static MoveSocketMessage InstantiateFromByteArray(byte[] data)
        {
            byte b = data[0]; // ClassId

            if (b != 101) throw new ArgumentException($"Data provided hasn't the right classId (provided: {b}, wanted: 101)", "data");

            return new MoveSocketMessage((data[2], data[4]), (data[6], data[8]));
        }
    }
}
