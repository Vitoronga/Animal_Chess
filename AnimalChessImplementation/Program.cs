using AnimalChessLibrary;
using System.Diagnostics;
using System.Text;

namespace AnimalChessImplementation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AnimalChess animalChess = new AnimalChess();

            while (!animalChess.IsFinished)
            {
                ShowMap(animalChess);

                MoveResult moveResult;

                do
                {
                    Console.Write("Origin square: ");
                    string originSquare = Console.ReadLine() ?? "";
                    Console.Write("Direction: ");
                    string direction = Console.ReadLine() ?? "";

                    (int, int) convertedOriginSquare = GetPositionTupleFromString(originSquare);
                    MoveDirection convertedDirection = GetDirectionFromString(direction);

                    moveResult = animalChess.MovePiece(convertedOriginSquare, convertedDirection);
                    Console.WriteLine(moveResult.Success ? "success" : "failed");
                } while (!moveResult.Success);

                Console.WriteLine(moveResult.ToString());
                Console.WriteLine("Selected Piece player: " + (moveResult.OriginPiece.Player1Piece ? "P1" : "P2"));
                Console.WriteLine("Game ended: " + animalChess.IsFinished);
            }
        }

        static void ShowMap(AnimalChess animalChess) => ShowMap(animalChess.BoardPieces, animalChess.BoardStructures);
        static void ShowMap(Piece[,] pieceMap, StructureTypes[,] structureMap)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("PieceMap:");

            for (int y = 0; y < pieceMap.GetLength(1); y++)
            {
                for (int x = 0; x < pieceMap.GetLength(0); x++)
                {
                    builder.Append($"{pieceMap[x, pieceMap.GetLength(1) - 1 - y]?.PieceType ?? PieceTypes.None}     \t");
                }

                builder.AppendLine();
            }

            builder.AppendLine("\nStructureMap:");

            for (int y = 0; y < structureMap.GetLength(1); y++)
            {
                for (int x = 0; x < structureMap.GetLength(0); x++)
                {
                    builder.Append($"{structureMap[x, structureMap.GetLength(1) - 1 - y]}\t");
                }

                builder.AppendLine();
            }

            Console.WriteLine(builder.ToString());
        }

        static void OldShowMap(Piece[,] pieceMap, StructureTypes[,] structureMap)
        {
            Console.WriteLine("PieceMap:");

            for (int y = 0; y < pieceMap.GetLength(1); y++)
            {
                for (int x = 0; x < pieceMap.GetLength(0); x++)
                {
                    Console.Write($"{pieceMap[x, pieceMap.GetLength(1) - 1 - y]?.PieceType ?? PieceTypes.None}     \t");
                }

                Console.WriteLine();
            }

            Console.WriteLine("\nStructureMap:");
            
            for (int y = 0; y < structureMap.GetLength(1); y++)
            {
                for (int x = 0; x < structureMap.GetLength(0); x++)
                {
                    Console.Write($"{structureMap[x, structureMap.GetLength(1) - 1 - y]}\t");
                }

                Console.WriteLine();
            }
        }

        static (int, int) GetPositionTupleFromString(string square)
        {
            if (!square.Contains(',')) throw new ArgumentException("Input format is invalid", "square");

            int x, y;
            if (!int.TryParse(square.Substring(0, square.IndexOf(',')), out x)) throw new ArgumentException("Input can't be parsed to a numeric value");
            if (!int.TryParse(square.Substring(square.IndexOf(',') + 1), out y)) throw new ArgumentException("Input can't be parsed to a numeric value");

            return (x, y);
        }

        static MoveDirection GetDirectionFromString(string input)
        {
            input = input.ToLower();

            if (input.Contains("up") || input[0] == 'u') return MoveDirection.Up;
            else if (input.Contains("down") || input[0] == 'd') return MoveDirection.Down;
            else if (input.Contains("left") || input[0] == 'l') return MoveDirection.Left;
            else if (input.Contains("right") || input[0] == 'r') return MoveDirection.Right;
            else throw new ArgumentException("Couldn't translate input into a MoveDirection value", "input");
        }

        static void DebugTests()
        {
            Console.ReadLine();
            Stopwatch sw = Stopwatch.StartNew();
            AnimalChess animalChess = new AnimalChess();
            sw.Stop();
            Console.WriteLine($"Time elapsed for class initialization: {sw.Elapsed.TotalMilliseconds}ms");

            while (!animalChess.IsFinished)
            {
                sw.Restart();
                OldShowMap(animalChess.BoardPieces, animalChess.BoardStructures);
                sw.Stop();
                Console.WriteLine($"\nTime elapsed for map printings [OLD]: {sw.Elapsed.TotalMilliseconds}ms");

                Console.WriteLine("\n\n");

                sw.Restart();
                ShowMap(animalChess.BoardPieces, animalChess.BoardStructures);
                sw.Stop();
                Console.WriteLine($"\nTime elapsed for map printings [NEW]: {sw.Elapsed.TotalMilliseconds}ms");

                Console.ReadLine();
            }
        }
    }
}