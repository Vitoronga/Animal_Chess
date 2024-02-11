using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalChessLibrary
{
    public static class PieceFactory
    {
        public static Piece GeneratePiece(PieceTypes pieceType, bool player1Piece)
        {
            Piece piece;

            switch(pieceType)
            {
                // 1
                case PieceTypes.Rat:
                    piece = new Piece(PieceTypes.Rat, player1Piece, false, false, true);
                    break;
                
                // 2 - 5
                case PieceTypes.Cat:
                case PieceTypes.Dog:
                case PieceTypes.Wolf:
                case PieceTypes.Leopard:
                    piece = new Piece(pieceType, player1Piece, false, false, false);
                    break;

                // 6
                case PieceTypes.Tiger:
                    piece = new Piece(pieceType, player1Piece, true, false, false);
                    break;

                // 7
                case PieceTypes.Lion:
                    piece = new Piece(pieceType, player1Piece, true, true, false);
                    break;
                
                // 8
                case PieceTypes.Elephant:
                    piece = new Piece(pieceType, player1Piece, false, false, false);
                    break;

                default:
                    throw new ArgumentException("The provided piece type doesn't exist in this context", "pieceType");
            }

            return piece;
        }
    }
}
