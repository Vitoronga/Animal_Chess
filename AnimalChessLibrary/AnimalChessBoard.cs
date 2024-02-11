using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalChessLibrary
{
    public enum StructureTypes
    {
        Empty = 0,
        Water = 1,
        P1Trap = 2,
        P2Trap = 3,
        P1Den = 4,
        P2Den = 5
    }

    public enum MoveDirection
    {
        Right = 0,
        Up = 1,
        Left = 2,
        Down = 3
    }

    internal class AnimalChessBoard
    {
        private StructureTypes[,] structuresMap;
        private Piece[,] piecesMap;

        public IEnumerable<StructureTypes> StructuresMap { 
            get { 
                return CollectionHelper.GetListFromBidimensionalArray(structuresMap); 
            }
        }

        public IEnumerable<Piece> PiecesMap {
            get {
                return CollectionHelper.GetListFromBidimensionalArray(piecesMap);
            }
        }

        public AnimalChessBoard()
        {
            structuresMap = new StructureTypes[7, 9];
            piecesMap = new Piece[7, 9];

            RegenerateStructures();
            RegeneratePieces();
        }

        public MoveResult MovePiece((int x, int y) originPos, MoveDirection direction, bool player1Turn)
        {
            (int x, int y) targetPos;
            try
            {
                targetPos = ValidateMove(originPos, direction, player1Turn);
            } catch (Exception e)
            {
                //throw;
                return new MoveResult(false, e.Message, piecesMap[originPos.x, originPos.y], null, direction, originPos, null);
            }


            // Generate move result
            MoveResult result = new MoveResult(true, "Piece moved successfully", piecesMap[originPos.x, originPos.y], piecesMap[targetPos.x, targetPos.y], direction, originPos, targetPos);
            
            // Play the move
            piecesMap[targetPos.x, targetPos.y] = piecesMap[originPos.x, originPos.y];
            piecesMap[originPos.x, originPos.y] = null;

            return result;
        }

        private (int, int) ValidateMove((int x, int y) originPos, MoveDirection direction, bool player1Turn)
        {
            // ----- Validating input -----

            // Check out of bounds origin pos
            if (originPos.x < 0 || originPos.x > piecesMap.GetLength(0) ||
                originPos.y < 0 || originPos.y > piecesMap.GetLength(1))
            {
                throw new ArgumentException("Invalid origin position", "originPos");
            }

            Piece originPiece = piecesMap[originPos.x, originPos.y];

            // Check origin piece existence
            if (originPiece == null)
                throw new ArgumentException("There is no piece at the provided location", "location");

            // Check if piece ownership is right
            if (originPiece.Player1Piece != player1Turn)
                throw new InvalidOperationException("This piece doesn't belong to the current player");

            (int x, int y) dir = MathHelper.AngleToCosSinTuple((int)direction * 90);
            (int x, int y) targetPos = (originPos.x + dir.x, originPos.y + dir.y);
            Piece targetPiece = piecesMap[targetPos.x, targetPos.y];

            // Check out of bounds direction
            if (targetPos.x < 0 || targetPos.x > piecesMap.GetLength(0) ||
                targetPos.y < 0 || targetPos.y > piecesMap.GetLength(1))
            {
                throw new ArgumentException("Can't move piece to the provided direction", "direction");
            }


            // ----- Validating game logic -----


            // Check if target is water (Swim/Jump)
            if (structuresMap[targetPos.x, targetPos.y] == StructureTypes.Water)
            {
                if (!originPiece.CanSwim && !originPiece.CanJump) 
                    throw new InvalidOperationException("This piece can't enter or jump over water");
                else if (originPiece.CanJump)
                {
                    // Can jump in that direction?
                    if (((direction == MoveDirection.Up || direction == MoveDirection.Down) && !originPiece.CanJumpVertically) &&
                        ((direction == MoveDirection.Left || direction == MoveDirection.Right) && !originPiece.CanJumpHorizontally))
                        throw new InvalidOperationException("Piece can't jump in that direction");

                    (int x, int y) newTargetPos = targetPos;

                    // Find obstacles (another piece) in the path of water
                    while (structuresMap[newTargetPos.x, newTargetPos.y] == StructureTypes.Water)
                    {
                        if (piecesMap[newTargetPos.x, newTargetPos.y] != null)
                            throw new InvalidOperationException("Can't jump over water because a piece is in the way");

                        newTargetPos = (newTargetPos.x + dir.x, newTargetPos.y + dir.y);
                    }

                    // Update new target
                    targetPos = newTargetPos;
                    targetPiece = piecesMap[targetPos.x, targetPos.y];
                }
            }


            // Check if space is occupied... (Captures)
            if (targetPiece != null)
            {
                // ... by allied piece
                if (targetPiece.Player1Piece == originPiece.Player1Piece)
                    throw new InvalidOperationException("Target location is already occupied");
                // ... by enemy piece
                else
                {
                    // Is piece capturing from water to land?
                    if (structuresMap[originPos.x, originPos.y] == StructureTypes.Water &&
                        structuresMap[targetPos.x, targetPos.y] == StructureTypes.Empty)
                        throw new InvalidOperationException("Can't capture pieces from water to land");

                    // Is piece capturing from land to water?
                    if (structuresMap[originPos.x, originPos.y] == StructureTypes.Empty &&
                        structuresMap[targetPos.x, targetPos.y] == StructureTypes.Water)
                        throw new InvalidOperationException("Can't capture pieces from land to water");

                    int originPieceStrength = (int)originPiece.PieceType;
                    int targetPieceStrength = (int)targetPiece.PieceType;

                    // Check capture scenarios
                    if (!(targetPiece.IsTrapped ||
                         (originPieceStrength == 1 && targetPieceStrength == 8) ||
                         (originPieceStrength >= targetPieceStrength)))
                    {
                        throw new InvalidOperationException("Origin piece isn't strong enough to capture target");
                    }
                }
            }

            // Check if space has an enemy trap
            if ((structuresMap[targetPos.x, targetPos.y] == StructureTypes.P1Trap && !originPiece.Player1Piece) ||
                (structuresMap[targetPos.x, targetPos.y] == StructureTypes.P2Trap && originPiece.Player1Piece))
            {
                originPiece.IsTrapped = true;
            } else if (originPiece.IsTrapped) originPiece.IsTrapped = false;


            // Actually play the move (MOVED TO CALLER)
            //piecesMap[targetPos.x, targetPos.y] = originPiece;
            //piecesMap[originPos.x, originPos.y] = null;
            return targetPos;
        }

        public (bool, bool) HasGameEndedAndPlayerOneWon()
        {
            for (int i = 0; i < structuresMap.GetLength(0); i++)
            {
                for (int j = 0; j < structuresMap.GetLength(1); j++)
                {
                    if (structuresMap[i, j] == StructureTypes.P1Den && (!piecesMap[i, j]?.Player1Piece ?? false)) // crazy one-liner xd
                        return (true, false);
                    else if (structuresMap[i, j] == StructureTypes.P2Den && (piecesMap[i, j]?.Player1Piece ?? false))
                        return (true, true);
                }
            }

            return (false, false);
        }

        public void RegeneratePieces()
        {
            // Starting from bottom-left
            piecesMap[0, 0] = PieceFactory.GeneratePiece(PieceTypes.Tiger, true);
            piecesMap[6, 0] = PieceFactory.GeneratePiece(PieceTypes.Lion, true);
            piecesMap[1, 1] = PieceFactory.GeneratePiece(PieceTypes.Cat, true);
            piecesMap[5, 1] = PieceFactory.GeneratePiece(PieceTypes.Dog, true);
            piecesMap[0, 2] = PieceFactory.GeneratePiece(PieceTypes.Elephant, true);
            piecesMap[2, 2] = PieceFactory.GeneratePiece(PieceTypes.Wolf, true);
            piecesMap[4, 2] = PieceFactory.GeneratePiece(PieceTypes.Leopard, true);
            piecesMap[6, 2] = PieceFactory.GeneratePiece(PieceTypes.Rat, true);

            piecesMap[0, 6] = PieceFactory.GeneratePiece(PieceTypes.Rat, false);
            piecesMap[2, 6] = PieceFactory.GeneratePiece(PieceTypes.Leopard, false);
            piecesMap[4, 6] = PieceFactory.GeneratePiece(PieceTypes.Wolf, false);
            piecesMap[6, 6] = PieceFactory.GeneratePiece(PieceTypes.Elephant, false);
            piecesMap[1, 7] = PieceFactory.GeneratePiece(PieceTypes.Dog, false);
            piecesMap[5, 7] = PieceFactory.GeneratePiece(PieceTypes.Cat, false);
            piecesMap[0, 8] = PieceFactory.GeneratePiece(PieceTypes.Lion, false);
            piecesMap[6, 8] = PieceFactory.GeneratePiece(PieceTypes.Tiger, false);
        }

        public void RegenerateStructures()
        {
            // Starting from bottom-left

            structuresMap[2, 0] = StructureTypes.P1Trap;
            structuresMap[3, 0] = StructureTypes.P1Den;
            structuresMap[4, 0] = StructureTypes.P1Trap;
            structuresMap[3, 1] = StructureTypes.P1Trap;

            structuresMap[1, 3] = StructureTypes.Water;
            structuresMap[2, 3] = StructureTypes.Water;
            structuresMap[4, 3] = StructureTypes.Water;
            structuresMap[5, 3] = StructureTypes.Water;
            structuresMap[1, 4] = StructureTypes.Water;
            structuresMap[2, 4] = StructureTypes.Water;
            structuresMap[4, 4] = StructureTypes.Water;
            structuresMap[5, 4] = StructureTypes.Water;
            structuresMap[1, 5] = StructureTypes.Water;
            structuresMap[2, 5] = StructureTypes.Water;
            structuresMap[4, 5] = StructureTypes.Water;
            structuresMap[5, 5] = StructureTypes.Water;

            structuresMap[3, 7] = StructureTypes.P2Trap;
            structuresMap[2, 8] = StructureTypes.P2Trap;
            structuresMap[3, 8] = StructureTypes.P2Den;
            structuresMap[4, 8] = StructureTypes.P2Trap;
        }
    }
}
