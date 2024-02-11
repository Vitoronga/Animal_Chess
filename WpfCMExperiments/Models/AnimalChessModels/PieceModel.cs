using AnimalChessLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCMExperiments.Models.AnimalChessModels
{
    public class PieceModel
    {
        public Piece BasePiece { get; init; }
        public string ImagePath { get; init; }
        public int PieceStrength => (int)BasePiece.PieceType;
        public bool IsTeam1Piece => BasePiece.Player1Piece;
        public bool IsTeam2Piece => !IsTeam1Piece;

        public PieceModel(Piece basePiece, string imagePath)
        {
            BasePiece = basePiece;
            ImagePath = imagePath;
        }
    }
}
