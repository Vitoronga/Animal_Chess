using AnimalChessLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCMExperiments.Models.AnimalChessModels
{
    public static class AnimalChessModelFactory
    {
        private static readonly string MAIN_IMAGE_FOLDER = "/Images/AnimalChessImages/";

        private static readonly Dictionary<PieceTypes, string> PIECE_IMAGE_PATHS = new Dictionary<PieceTypes, string>()
        {
            [PieceTypes.Rat] = MAIN_IMAGE_FOLDER + "rat.png",
            [PieceTypes.Cat] = MAIN_IMAGE_FOLDER + "cat.png",
            [PieceTypes.Dog] = MAIN_IMAGE_FOLDER + "dog.png",
            [PieceTypes.Wolf] = MAIN_IMAGE_FOLDER + "wolf.png",
            [PieceTypes.Leopard] = MAIN_IMAGE_FOLDER + "leopard.png",
            [PieceTypes.Tiger] = MAIN_IMAGE_FOLDER + "tiger.png",
            [PieceTypes.Lion] = MAIN_IMAGE_FOLDER + "lion.png",
            [PieceTypes.Elephant] = MAIN_IMAGE_FOLDER + "elephant.png"
        };

        private static readonly Dictionary<StructureTypes, string> STRUCTURE_IMAGE_PATHS = new Dictionary<StructureTypes, string>()
        {
            [StructureTypes.Empty] = MAIN_IMAGE_FOLDER + "empty.png",
            [StructureTypes.Water] = MAIN_IMAGE_FOLDER + "water.png",
            [StructureTypes.P1Trap] = MAIN_IMAGE_FOLDER + "p1_trap.png",
            [StructureTypes.P2Trap] = MAIN_IMAGE_FOLDER + "p2_trap.png",
            [StructureTypes.P1Den] = MAIN_IMAGE_FOLDER + "p1_den.png",
            [StructureTypes.P2Den] = MAIN_IMAGE_FOLDER + "p2_den.png"
        };

        public static PieceModel? GeneratePieceModel(Piece basePiece)
        {
            if (basePiece == null) return null;

            return new PieceModel(basePiece, PIECE_IMAGE_PATHS[basePiece.PieceType]);
        }

        public static StructureModel? GenerateStructureModel(StructureTypes baseStructure)
        {
            //if (structure == null) return null; // Can't be null, and there is already a enum type for no structure

            return new StructureModel(baseStructure, STRUCTURE_IMAGE_PATHS[baseStructure]);
        }

        public static TileModel GenerateTileModel(Piece basePiece, StructureTypes baseStructure, int positionX, int positionY)
                                => GenerateTileModel(GeneratePieceModel(basePiece), GenerateStructureModel(baseStructure), (positionX, positionY));
        public static TileModel GenerateTileModel(Piece basePiece, StructureTypes baseStructure, (int, int) position) 
                                => GenerateTileModel(GeneratePieceModel(basePiece), GenerateStructureModel(baseStructure), position);
        public static TileModel GenerateTileModel(PieceModel? pieceModel, StructureModel? structureModel, int positionX, int positionY) 
                                => GenerateTileModel(pieceModel, structureModel, (positionX, positionY));
        public static TileModel GenerateTileModel(PieceModel? pieceModel, StructureModel? structureModel, (int, int) position)
        {
            return new TileModel(pieceModel, structureModel, position);
        }
    }
}
