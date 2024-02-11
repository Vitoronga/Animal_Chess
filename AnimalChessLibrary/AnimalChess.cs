using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AnimalChessLibrary
{
    internal enum GameStatus
    {
        Initial = 0,
        Playing = 1,
        Ended = 2
    }

    public class AnimalChess
    {
        private AnimalChessBoard board;
        private GameStatus status;
        public bool Player1Turn { get; private set; }

        public bool IsIdle => status == GameStatus.Initial;
        public bool IsPlaying => status == GameStatus.Playing;
        public bool IsFinished => status == GameStatus.Ended;
        public bool PlayerOneWon => board.HasGameEndedAndPlayerOneWon().Item2;

        //public IEnumerable<Piece> BoardPieces => board.PiecesMap;
        //public IEnumerable<Structures> BoardStructures => board.StructuresMap;

        public Piece[,] BoardPieces => CollectionHelper.GetBidimensionalArrayFromList(board.PiecesMap.ToList(), 7, 9);
        public StructureTypes[,] BoardStructures => CollectionHelper.GetBidimensionalArrayFromList(board.StructuresMap.ToList(), 7, 9);


        public AnimalChess()
        {
            board = new AnimalChessBoard();
            Player1Turn = true;
        }

        public void ResetGame(bool forceGameTermination = false)
        {
            if (forceGameTermination) status = GameStatus.Ended;

            ResetBoard();
            ResetGameLogicContext();
        }

        public MoveResult MovePiece((int x, int y) origin, (int x, int y) target)
        {
            MoveDirection dir;

            try
            {
                dir = GetDirectionFromCoordinates(target.x - origin.x, target.y - origin.y);
            }
            catch (Exception e)
            {
                return new MoveResult(false, e.Message, null, null, null, null, null);
            }

            return MovePiece(origin, dir);
        }
        public MoveResult MovePiece((int x, int y) origin, MoveDirection direction)
        {
            if (IsFinished) return new MoveResult(false, "Game has already ended.", null, null, null, null, null);

            MoveResult result = board.MovePiece(origin, direction, Player1Turn);

            // Set playing status if needed
            if (result.Success)
            {
                if (!IsPlaying) status = GameStatus.Playing;

                Player1Turn = !Player1Turn;
            }

            // Check if game ended
            if (board.HasGameEndedAndPlayerOneWon().Item1) status = GameStatus.Ended;

            return result;
        }

        // Internal Controllers

        private void ResetBoard()
        {
            if (IsPlaying) throw new InvalidOperationException("Can't reset the board from a game in progress.");

            // Set board to default
            board.RegeneratePieces();
            board.RegenerateStructures();
        }

        private void ResetGameLogicContext()
        {
            status = GameStatus.Initial;
            Player1Turn = true;
        }

        // Internal Handlers

        private MoveDirection GetDirectionFromCoordinates(int x, int y)
        {
            if (x != 0 && y != 0) throw new ArgumentOutOfRangeException("y", $"Can't move piece diagonally ({x},{y})");
            else if (x == 0 && y == 0) throw new ArgumentOutOfRangeException("y", "Coordinates can't be both 0");

            x = Math.Clamp(x, -1, 1);
            y = Math.Clamp(y, -1, 1);

            if (x == 1) return MoveDirection.Right;
            else if (x == -1) return MoveDirection.Left;
            else if (y == 1) return MoveDirection.Up;
            else if (y == -1) return MoveDirection.Down;
            else throw new InvalidOperationException("Couldn't generate direction");
        }
    }
}
