using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalChessLibrary
{
    public enum PieceTypes
    {
        None = 0,
        Rat = 1,
        Cat = 2,
        Dog = 3,
        Wolf = 4,
        Leopard = 5,
        Tiger = 6,
        Lion = 7,
        Elephant = 8
    }

    public class Piece
    {
        public PieceTypes PieceType { get; init; }
        public bool Player1Piece { get; init; }
        public bool CanJumpVertically { get; init; }
        public bool CanJumpHorizontally { get; init; }
        public bool CanJump => CanJumpVertically || CanJumpHorizontally;
        public bool CanSwim { get; init; }
        public bool IsTrapped { get; set; }
        public string PieceTypeSymbol => PieceType.ToString().Substring(0, PieceType == (PieceTypes)7 ? 2 : 1);

        internal Piece(PieceTypes pieceType, bool player1Piece, bool canJumpVertically, bool canJumpHorizontally, bool canSwim)
        {
            PieceType = pieceType;
            Player1Piece = player1Piece;
            CanJumpVertically = canJumpVertically;
            CanJumpHorizontally = canJumpHorizontally;
            CanSwim = canSwim;
        }
    }
}
