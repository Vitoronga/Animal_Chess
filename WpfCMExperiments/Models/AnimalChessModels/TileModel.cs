using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfCMExperiments.Models.AnimalChessModels
{
    public class TileModel
    {
        public PieceModel? Piece { get; init; }
        public bool HasPiece => Piece != null;
        public Visibility PieceVisibility => (HasPiece ? Visibility.Visible : Visibility.Hidden);

        public StructureModel? Structure { get; init; }
        public bool HasStructure => Structure != null;
        public Visibility StructureVisibility => (HasStructure ? Visibility.Visible : Visibility.Hidden);

        public (int x, int y) Position { get; set; }

        public Visibility Team1MarkerVisibility => HasPiece ? (Piece.IsTeam1Piece ? Visibility.Visible : Visibility.Hidden) : Visibility.Hidden;
        public Visibility Team2MarkerVisibility => HasPiece ? (Piece.IsTeam2Piece ? Visibility.Visible : Visibility.Hidden) : Visibility.Hidden;

        private bool selected = false;
        public Visibility SelectedHighlight => selected ? Visibility.Visible : Visibility.Hidden;

        public TileModel(PieceModel? piece, StructureModel? structure, int positionX, int positionY) : this(piece, structure, (positionX, positionY)) { }

        public TileModel(PieceModel? piece, StructureModel? structure, (int, int) position)
        {
            Piece = piece;
            Structure = structure;
            Position = position;
        }

        public void ToggleSelection(bool value)
        {
            selected = value;
        }
    }
}
