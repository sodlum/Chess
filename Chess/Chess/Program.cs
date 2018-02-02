using Chess.Game;
using Chess.Pieces;
using System.Linq;
using System;

namespace Chess {
    class Program {
        static void Main(string[] args) {
            var board = new Board();
            board.Start();

            WriteBoardToConsole(board);

            var moves = new System.Collections.Generic.List<MoveWasSuccess>();

            //Pawns
            moves.Add(MovePiece(board, "G1", "E1"));
            moves.Add(MovePiece(board, "G2", "E2"));
            moves.Add(MovePiece(board, "G3", "E3"));
            moves.Add(MovePiece(board, "G4", "E4"));
            moves.Add(MovePiece(board, "G5", "E5"));
            moves.Add(MovePiece(board, "G6", "E6"));
            moves.Add(MovePiece(board, "G7", "E7"));
            moves.Add(MovePiece(board, "G8", "E8"));
            moves.Add(MovePiece(board, "E1", "D1"));
            moves.Add(MovePiece(board, "E2", "D2"));
            moves.Add(MovePiece(board, "E3", "D3"));
            moves.Add(MovePiece(board, "E4", "D4"));
            moves.Add(MovePiece(board, "E5", "D5"));
            moves.Add(MovePiece(board, "E6", "D6"));
            moves.Add(MovePiece(board, "E7", "D7"));
            moves.Add(MovePiece(board, "E8", "D8"));
            moves.Add(MovePiece(board, "D1", "C1"));
            moves.Add(MovePiece(board, "D2", "C2"));
            moves.Add(MovePiece(board, "D3", "C3"));
            moves.Add(MovePiece(board, "D4", "C4"));
            moves.Add(MovePiece(board, "D5", "C5"));
            moves.Add(MovePiece(board, "D6", "C6"));
            moves.Add(MovePiece(board, "D7", "C7"));
            moves.Add(MovePiece(board, "D8", "C8"));
            //Knights
            moves.Add(MovePiece(board, "H2", "F3"));
            moves.Add(MovePiece(board, "F3", "E1"));
            moves.Add(MovePiece(board, "H7", "F6"));
            moves.Add(MovePiece(board, "F6", "E8"));
            //Rooks
            moves.Add(MovePiece(board, "H1", "F1"));
            moves.Add(MovePiece(board, "F1", "G1"));
            moves.Add(MovePiece(board, "G1", "G8"));
            moves.Add(MovePiece(board, "G8", "G1"));
            moves.Add(MovePiece(board, "H8", "F8"));
            moves.Add(MovePiece(board, "F8", "G8"));
            moves.Add(MovePiece(board, "F8", "F1"));
            moves.Add(MovePiece(board, "F1", "F7"));
            //Bishops
            moves.Add(MovePiece(board, "H3", "D7"));
            moves.Add(MovePiece(board, "D7", "E8"));
            moves.Add(MovePiece(board, "E8", "D7"));
            moves.Add(MovePiece(board, "D7", "H3"));
            moves.Add(MovePiece(board, "H6", "D2"));
            moves.Add(MovePiece(board, "D2", "E1"));
            moves.Add(MovePiece(board, "E1", "D2"));
            moves.Add(MovePiece(board, "D2", "H6"));
            //Queen
            moves.Add(MovePiece(board, "H4", "E4"));
            moves.Add(MovePiece(board, "E4", "E6"));
            moves.Add(MovePiece(board, "E6", "D5"));
            moves.Add(MovePiece(board, "D5", "D3"));
            moves.Add(MovePiece(board, "D3", "E4"));
            moves.Add(MovePiece(board, "E4", "F4"));
            //King
            moves.Add(MovePiece(board, "H5", "H4"));
            moves.Add(MovePiece(board, "H4", "H5"));
            moves.Add(MovePiece(board, "H5", "G6"));
            moves.Add(MovePiece(board, "G6", "H7"));
            moves.Add(MovePiece(board, "H7", "G6"));
            moves.Add(MovePiece(board, "G6", "H5"));
            //Other Pawn
            moves.Add(MovePiece(board, "B8", "C7"));
            moves.Add(MovePiece(board, "B7", "C6"));
            moves.Add(MovePiece(board, "B6", "C5"));
            moves.Add(MovePiece(board, "B5", "C4"));
            moves.Add(MovePiece(board, "B4", "C3"));
            moves.Add(MovePiece(board, "B3", "C2"));
            moves.Add(MovePiece(board, "B2", "C1"));
            //Knight Again
            moves.Add(MovePiece(board, "E1", "C2"));
            moves.Add(MovePiece(board, "E8", "C7"));
            moves.Add(MovePiece(board, "C2", "A3"));
            moves.Add(MovePiece(board, "C7", "A8"));
            //Rook Again
            moves.Add(MovePiece(board, "G1", "B1"));
            //Pawn as Queen
            //moves.Add(MovePiece(board, "C8", "B7"));
            //moves.Add(MovePiece(board, "B7", "A6"));
            //moves.Add(MovePiece(board, "A6", "B7"));
            //moves.Add(MovePiece(board, "B7", "B6"));
            //moves.Add(MovePiece(board, "B6", "B8"));

            foreach (var move in moves) {
                Console.WriteLine(move.ToString());
            }
           
            //while (!IsGameOver(board)) {
            //    Console.Write("Input move as 'From|To': ");
            //    string input = Console.ReadLine();
            //    string[] inputs = input.Split('|');
            //    string from = inputs[0].ToUpper();
            //    string to = inputs[1].ToUpper();
            //    MovePiece(board, from, to);
            //}

            //var winningTeam = board.Pieces.SingleOrDefault(p => p != null && p.GetType() == typeof(King) && !(p as King).InCheckMate).Team;

            //Console.WriteLine("GAME OVER!");
            //Console.WriteLine($"Team {winningTeam} is the victor!");

            Console.Read();
        }

        static MoveWasSuccess MovePiece(Board board, string from, string to) {
            var move = new MoveWasSuccess(from, to);
            move.IsSuccess = board.MovePiece(from, to);
            WriteBoardToConsole(board);
            //Console.ReadLine();
            return move;
        }

        static bool IsGameOver(Board board) {
            return board.Pieces.Any(p => p != null && p.GetType() == typeof(King) && (p as King).InCheckMate);
        }

        static void WriteBoardToConsole(Board chessBoard) {
            Console.Clear();

            Console.WriteLine("  |  1 |  2 |  3 |  4 |  5 |  6 |  7 |  8 | ");
            Console.WriteLine("--------------------------------------------");

            var x_index = 8;
            var x = 1;

            foreach(var tile in chessBoard.Tiles) {
                if (x == 1) {
                    Console.Write($"{tile.Location[0]}");
                }

                Console.Write(" | ");
                if (tile.CurrentOccupant != null) {
                    Console.Write($"{tile.CurrentOccupant.Name}");
                } else {
                    Console.Write("  ");
                }

                if (x == x_index) {
                    Console.Write(" | ");
                    Console.WriteLine("");
                    Console.Write("--------------------------------------------");
                    Console.WriteLine("");
                    x = 0;
                }

                x++;
            }
        }
    }

    struct MoveWasSuccess {
        public string From { get; set; }
        public string To { get; set; }
        public bool IsSuccess { get; set; }

        public MoveWasSuccess(string from, string to) {
            From = from;
            To = to;
            IsSuccess = false;
        }

        public override string ToString() {
            return $"Move from {this.From} to {this.To} was{(this.IsSuccess ? "" : " not")} successful.";
        }
    }
}