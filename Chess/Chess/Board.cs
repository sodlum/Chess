using Chess.Collections;
using Chess.Pieces;
using Chess.Tiles;
using System;
using System.Linq;

namespace Chess.Game {
    /// <summary>
    /// Chess.Game.Board object to represent the chess game.
    /// </summary>
    class Board {
        private const int ROW_AND_COLUMN_LENGTH = 8;
        private Team _current_turn;

        /// <summary>
        /// Collection of tiles representing the chess board.
        /// </summary>
        public TileCollection Tiles { get; private set; }

        /// <summary>
        /// Collection of pieces currently on the chess board.
        /// </summary>
        public PieceCollection Pieces { get; private set; }

        /// <summary>
        /// Collection of moves taken by all pieces on the chess board.
        /// </summary>
        public MoveCollection MovesTaken { get; private set; }

        /// <summary>
        /// Creates new instance of chess board.
        /// </summary>
        /// <param name="start">Optional flag to immediately start chess game.</param>
        public Board(bool start = false) {
            Pieces = new PieceCollection();
            Tiles = new TileCollection();
            MovesTaken = new MoveCollection();

            if (start) Start();
        }

        /// <summary>
        /// Starts the chess game.
        /// </summary>
        public void Start() {
            addPieces();
            _current_turn = Team.White;
        }

        /// <summary>
        /// Moves identified piece to the given location.
        /// </summary>
        /// <param name="piece">The piece to be moved.</param>
        /// <param name="to">The location to move to.</param>
        /// <returns>True if piece is successfully moved.</returns>
        public bool MovePiece(Piece piece, string to) {
            string location = $"{piece.Y}{piece.X}";
            return MovePiece(location, to);
        }

        /// <summary>
        /// Moves piece at <paramref name="from"/> position to <paramref name="to"/> position.
        /// </summary>
        /// <param name="from">Original location for the piece being moved.</param>
        /// <param name="to">The location the piece is moving to.</param>
        /// <returns>True if piece is successfully moved.</returns>
        public bool MovePiece(string from, string to) {
            var from_tile = this.Tiles[from];
            var to_tile = this.Tiles[to];
            var piece = from_tile.CurrentOccupant;

            if (piece == null) {
                return false;
            }

            //if (piece.Team != _current_turn) {
            //    return false;
            //}

            if(!piece.MovePiece(to)) {
                return false;
            }

            from_tile.CurrentOccupant = null;

            if (piece.IsTakingPiece) {
                this.Pieces.RemovePiece(to_tile.CurrentOccupant);
            }

            piece.IsTakingPiece = false;
            to_tile.CurrentOccupant = piece;

            var move = piece.MovesTaken[piece.MovesTaken.TotalMoves - 1];
            this.MovesTaken.AddMove(move);

            var opposingKing = (King)this.Pieces.SingleOrDefault(p => p != null && p.Team != _current_turn && typeof(King) == p.GetType());

            foreach (var pce in this.Pieces.Where(p => p != null && p.Team == _current_turn && typeof(King) != p.GetType())) {
                pce.SetFlags = false;

                if (!pce.CanMovePiece(opposingKing.X, opposingKing.Y).IsBlank()) {
                    opposingKing.IsInCheck = true;
                }

                pce.SetFlags = true;
            }
            
            if (opposingKing.IsInCheck) {
                foreach(var tile in opposingKing.NeighboringSpots) {
                    var x = Convert.ToInt32(tile.Location[1]);
                    var y = tile.Location[0];

                    if (opposingKing.CanMovePiece(x, y).IsBlank()) {
                        opposingKing.InCheckMate = true;
                    }
                }
            }

            //if (_current_turn == Team.White) {
            //    _current_turn = Team.Black;
            //} else {
            //    _current_turn = Team.White;
            //}

            return true;
        }

        /// <summary>
        /// Add tiles to board. Add pieces after tiles are added.
        /// </summary>
        private void addTiles() {
            var y = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

            foreach(var c in y) {
                for(var i = 1; i <= ROW_AND_COLUMN_LENGTH; i++) {
                    var l = $"{c}{i}";
                    this.Tiles.AddTile(new Tile(l));
                }
            }

            addPieces();
        }

        /// <summary>
        /// Add pieces to board. Add tiles if not currently present.
        /// </summary>
        private void addPieces() {
            if(Tiles.TotalTiles == 0) {
                addTiles();
            } else {
                var a = new char[] { 'A', 'B', 'G', 'H' };
                var b = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };

                for(var i = 0; i < a.Length; i++) {
                    for(var j = 0; j < b.Length; j++) {
                        var piece = addPiece(a[i], b[j]);

                        this.Tiles[$"{a[i]}{b[j]}"].CurrentOccupant = piece;
                        this.Pieces.AddPiece(piece);
                    }
                }
            }
        }

        /// <summary>
        /// Create individual piece object based on params <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        /// <param name="y">Vertical location for piece.</param>
        /// <param name="x">Horizontal location for piece.</param>
        /// <returns>Piece object being created.</returns>
        private Piece addPiece(char y, int x) {
            Piece piece = null;

            if(y == 'B' || y == 'G') {
                piece = new Pawn($"{y}{x}", (y == 'B' ? Team.Black : Team.White), this);
            } else {
                var team = y == 'A' ? Team.Black : Team.White;
                var location = $"{y}{x}";

                if(x == 1 || x == 8) {
                    piece = new Rook(location, team, this);
                } else if(x == 2 || x == 7) {
                    piece = new Knight(location, team, this);
                } else if(x == 3 || x == 6) {
                    piece = new Bishop(location, team, this);
                } else if(x == 4) {
                    piece = new Queen(location, team, this);
                } else {
                    piece = new King(location, team, this);
                }
            }

            return piece;
        }
    }
}