using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalChessLibrary
{
    public class MoveResult
    {
        public bool Success { get; init; }
        public string Message { get; init; }
        public Piece? OriginPiece { get; init; }
        public Piece? TargetPiece { get; init; } // SET TARGETS IN THE BASE LOGIC
        public MoveDirection? Direction { get; init; }
        public (int, int)? OriginPosition { get; init; }
        public (int, int)? TargetPosition { get; init; }
        internal MoveResult(bool success, string message, Piece? originPiece, Piece? targetPiece, MoveDirection? direction, (int, int)? originPosition, (int, int)? targetPosition)
        {
            Success = success;
            Message = message;
            OriginPiece = originPiece;
            TargetPiece = targetPiece;
            Direction = direction;
            OriginPosition = originPosition;
            TargetPosition = targetPosition;
        }

        public override string ToString() => GetCompleteDescription();

        public string? GetMoveShortDescription()
        {
            if (Success) return $"{OriginPiece.PieceTypeSymbol.ToUpper()}{(TargetPiece != null ? 'x' : null)}{(char)(97 + TargetPosition.Value.Item1)}{TargetPosition.Value.Item2 + 1}";
            else return null;
        }

        public string GetCompleteDescription()
        {
            return $"Selected Piece: {OriginPiece?.PieceType}" +
                 $"\nTarget Piece: {TargetPiece?.PieceType}" +
                 $"\nSelected Direction: {Direction}" +
                 $"\nOrigin Position: {OriginPosition}" +
                 $"\nTarget Position: {TargetPosition}" +
                 $"\nResult: {(Success ? "success" : "failed")}" +
                 $"\nLog: {Message}";
        }
    }
}
