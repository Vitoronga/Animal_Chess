using AnimalChessLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCMExperiments.Models.AnimalChessModels
{
    public class AnimalChessModel
    {
        private AnimalChess animalChess;
        public bool GameFinished => animalChess.IsFinished;
        public bool GameInProgress => animalChess.IsPlaying;
        public bool PlayerOneWon => animalChess.PlayerOneWon;

        public AnimalChessModel()
        {
            animalChess = new AnimalChess();
        }

        // return the structure
        //public PieceModel[,] GetBoardPieceModels()
        //{
        //    Piece[,] boardPieces = animalChess.BoardPieces;
        //    PieceModel[,] pieceModels = new PieceModel[boardPieces.GetLength(0), boardPieces.GetLength(1)];

        //    for (int i = 0; i < pieceModels.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < pieceModels.GetLength(1); j++)
        //        {
        //            pieceModels[i, j] = AnimalChessModelFactory.GeneratePieceModel(boardPieces[i, j]);
        //        }
        //    }

        //    return pieceModels;
        //}

        //public StructureModel[,] GetStructureModels()
        //{
        //    StructureTypes[,] boardStructures = animalChess.BoardStructures;
        //    StructureModel[,] structureModels = new StructureModel[boardStructures.GetLength(0), boardStructures.GetLength(1)];

        //    for (int i = 0; i < structureModels.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < structureModels.GetLength(1); j++)
        //        {
        //            structureModels[i, j] = AnimalChessModelFactory.GenerateStructureModel(boardStructures[i, j]);
        //        }
        //    }

        //    return structureModels;
        //}

        public TileModel[,] GetTileModels()
        {
            Piece[,] boardPieces = animalChess.BoardPieces;
            StructureTypes[,] boardStructures = animalChess.BoardStructures;
            TileModel[,] tileModels = new TileModel[boardPieces.GetLength(0), boardPieces.GetLength(1)];

            for (int i = 0; i < tileModels.GetLength(0); i++)
            {
                for (int j = 0; j < tileModels.GetLength(1); j++)
                {
                    tileModels[i, j] = AnimalChessModelFactory.GenerateTileModel(boardPieces[i, j], boardStructures[i, j], (i, j));
                }
            }

            return tileModels;
        }


        public MoveResult PlayMove(int originX, int originY, int targetX, int targetY) => PlayMove((originX, originY), (targetX, targetY));
        public MoveResult PlayMove((int x, int y) origin, (int x, int y) target)
        {
            return animalChess.MovePiece(origin, target);
        }

        public void ResetGame(bool forceTermination = false) => animalChess.ResetGame(forceTermination);
    }
}
