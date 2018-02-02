using System;
using Chess.Game;
using Chess.Moves;
using Chess.Collections;
using Chess.Extensions;
using Chess.Exceptions;
using System.Linq;
using System.Collections.Generic;
using Chess.Tiles;

namespace Chess.Pieces {
    /// <summary>
    /// Chess.Pieces.Team
    /// enum to represent the two available Teams that each piece can have.
    /// </summary>
    public enum Team {
        White = 0,
        Black = 1
    }

    /// <summary>
    /// Chess.Pieces.Piece
    /// Object to represent common functionality between all pieces.
    /// </summary>
    abstract class Piece {
        protected const string STANDARD_RULE = "Standard";

        /// <summary>
        /// Available rules for each piece to decide how the Piece instance can move.
        /// </summary>
        protected Dictionary<string, Func<Move, bool>> MovementRules { get; private set; }

        /// <summary>
        /// Used for pieces to avoid setting flags when validating a pieces ability to move.
        /// </summary>
        public bool SetFlags { protected get; set; }

        /// <summary>
        /// The vertical location of the Piece instance.
        /// </summary>
        public char Y { get; set; }

        /// <summary>
        /// The horizontal location of the Piece instance.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The team of the Piece instance.
        /// </summary>
        public Team Team { get; private set; }

        /// <summary>
        /// Tracks the moves taken by the Piece instance.
        /// </summary>
        public MoveCollection MovesTaken { get; private set; }

        /// <summary>
        /// Reference to the chess board to be able to retrieve information about the surrounding pieces.
        /// </summary>
        public Board Parent { get; private set; }

        /// <summary>
        /// Flag to indicate whether or not this piece will be taking another piece with the current move.
        /// </summary>
        public bool IsTakingPiece { get; set; }

        /// <summary>
        /// Gets an identifier for the Piece so that it can be displayed on some output window.
        /// </summary>
        public abstract string Name { get; }

        public Guid ID { get; private set; }

        /// <summary>
        /// Creates a new instance of the Piece object.
        /// </summary>
        /// <param name="tile">The location that the Piece object will exist on the chess board.</param>
        /// <param name="team">The team that the Piece object will be on.</param>
        /// <param name="parent">The chess board.</param>
        public Piece(string tile, Team team, Board parent) {
            if (tile.Length == 2) {
                int i = 0;

                if (int.TryParse(tile[1].ToString(), out i)) {
                    var c = tile[0];

                    if (c.IsBetween('A', 'H')) {
                        Y = c;
                        X = i;
                    } else {
                        throw new InvalidTileException("Character parameter invalid for tile");
                    }
                } else {
                    throw new InvalidTileException("Number parameter invalid for tile");
                }
            } else {
                throw new InvalidTileException("Length parameter invalid for tile");
            }

            Team = team;
            MovesTaken = new MoveCollection();
            Parent = parent;
            MovementRules = new Dictionary<string, Func<Move, bool>>();
            ID = Guid.NewGuid();
            SetFlags = true;
        }

        public bool IsNotSame(Piece piece) {
            return piece != null && piece.ID != this.ID;
        }

        /// <summary>
        /// Moves the current instance of the Piece object to the given <paramref name="destination"/>.
        /// </summary>
        /// <param name="destination">The location to move the current Piece instance to.</param>
        /// <returns>True if the move was successful.</returns>
        public bool MovePiece(string destination) {
            int x;
            char y;

            if(destination.Length == 2) {
                int i = 0;

                if(int.TryParse(destination[1].ToString(), out i)) {
                    var c = destination[0];

                    if(c.IsBetween('A', 'H')) {
                        y = c;
                        x = i;
                    } else {
                        throw new InvalidTileException("Character parameter invalid for tile");
                    }
                } else {
                    throw new InvalidTileException("Number parameter invalid for tile");
                }
            } else {
                throw new InvalidTileException("Length parameter invalid for tile");
            }

            if (x == this.X && y == this.Y) {
                return false;
            }

            var occupant = this.Parent.Pieces[y, x];

            if (occupant != null) {
                if (occupant.Team != this.Team) {
                    this.IsTakingPiece = true;
                } else {
                    return false;
                }
            }

            var move = this.CanMovePiece(x, y);
            
            if (move.IsBlank()) {
                return false;
            } else {
                this.MovesTaken.AddMove(move);
                this.X = x;
                this.Y = y;
                return true;
            }
        }
        
        /// <summary>
        /// Common method to determine whether a piece can move under standard circumstances.
        /// </summary>
        /// <param name="x">Horizontal movement position.</param>
        /// <param name="y">Vertical movement position.</param>
        /// <returns>The Move object that is attempted when validating the movement rule.</returns>
        public virtual Move CanMovePiece(int x, char y) {
            var direction = this.getMovementDirection(x, y);
            var x_distance = ExtensionMethods.GetDistance(this.X, x);
            var y_distance = ExtensionMethods.GetDistance(this.Y, y);

            var attemptedMove = new Move(x_distance, y_distance, direction, this);

            if(this.MovementRules[STANDARD_RULE](attemptedMove)) {
                return attemptedMove;
            }

            return Move.Blank;
        }

        /// <summary>
        /// Gets the CardinalDirection that a move is going across the chess board.
        /// </summary>
        /// <param name="x">Horizontal movement position.</param>
        /// <param name="y">Vertical movement position.</param>
        /// <returns>The direction that the Piece instance will be moving.</returns>
        protected CardinalDirection getMovementDirection(int x, char y) {
            if(this.X > x && this.Y == y) {
                return CardinalDirection.West;
            } else if(this.X == x && this.Y < y) {
                return CardinalDirection.South;
            } else if(this.X < x && this.Y == y) {
                return CardinalDirection.East;
            } else if(this.X == x && this.Y > y) {
                return CardinalDirection.North;
            } else if(this.X > x && this.Y < y) {
                return (CardinalDirection.SouthWest);
            } else if(this.X > x && this.Y > y) {
                return (CardinalDirection.NorthWest);
            } else if(this.X < x && this.Y < y) {
                return (CardinalDirection.SouthEast);
            } else if(this.X < x && this.Y > y) {
                return (CardinalDirection.NorthEast);
            } else {
                return CardinalDirection.None;
            }
        }
    }

    /// <summary>
    /// Chess.Pieces.Pawn
    /// Represents the Pawn on the chess board.
    /// </summary>
    class Pawn : Piece {
        protected const string FIRST_MOVE_RULE = "First";
        protected const string TAKE_PIECE_RULE = "Take";
        protected const string QUEENS_RULE = "Queen";

        /// <summary>
        /// Flag to determine if the Pawn instance has made it to the other side of the chess board.
        /// </summary>
        public bool IsQueen { get; set; }

        /// <summary>
        /// Flag to determine if the Pawn instance has not moved yet allowing for a slightly different movement rule.
        /// </summary>
        public bool IsFirstMove { get; set; }
        
        /// <summary>
        /// Returns the name of the Pawn instance identified by the letter P and the first letter of the current Team.
        /// </summary>
        public override string Name {
            get {
                if (this.IsQueen) {
                    return $"Q{Team.ToString()[0]}";
                } else {
                    return $"P{Team.ToString()[0]}";
                }
            }
        }

        /// <summary>
        /// Creates a new instance of the Pawn class.
        /// </summary>
        /// <param name="tile">The location of the Pawn on the chess board.</param>
        /// <param name="team">The team that the Pawn is on.</param>
        /// <param name="parent">The chess board to allow referencing other Piece objects.</param>
        public Pawn(string tile, Team team, Board parent) : base(tile, team, parent) {
            IsFirstMove = true;
            MovementRules.Add(STANDARD_RULE, r => r.Horizonal == 0 && r.Vertical == 1 && r.Direction == (team == Team.Black ? CardinalDirection.South : CardinalDirection.North));
            MovementRules.Add(FIRST_MOVE_RULE, r => r.Horizonal == 0 && r.Vertical.IsBetween(1, 2) && r.Direction == (team == Team.Black ? CardinalDirection.South : CardinalDirection.North));
            MovementRules.Add(TAKE_PIECE_RULE, r => r.Horizonal == r.Vertical && r.Vertical == 1 && r.Direction == (team == Team.Black ? CardinalDirection.SouthWest : CardinalDirection.NorthWest) || r.Direction == (team == Team.Black ? CardinalDirection.SouthEast : CardinalDirection.NorthEast));
            MovementRules.Add(QUEENS_RULE, r => (r.Vertical == r.Horizonal) || (r.Horizonal > 0 && r.Vertical == 0) || (r.Horizonal == 0 && r.Vertical > 0));
        }

        /// <summary>
        /// Method to identify if the Pawn instance can move with the provided <paramref name="rule"/>.
        /// </summary>
        /// <param name="x">Horizontal movement position.</param>
        /// <param name="y">Vertical movement position.</param>
        /// <param name="rule">Rule to be applied when attempting the move.</param>
        /// <returns>The attempted Move.</returns>
        private Move canMoveWithRule(int x, char y, string rule) {
            var exceptionRule = this.MovementRules[rule];
            var direction = getMovementDirection(x, y);
            var x_distance = ExtensionMethods.GetDistance(this.X, x);
            var y_distance = ExtensionMethods.GetDistance(this.Y, y);

            var attemptedMove = new Move(x_distance, y_distance, direction, this);

            if(!exceptionRule(attemptedMove)) {
                attemptedMove = Move.Blank;
            }

            return attemptedMove;
        }
        
        /// <summary>
        /// Determines whether the Pawn instance can move under any special cirumstances.
        /// </summary>
        /// <param name="x">Horizontal movement position.</param>
        /// <param name="y">Vertical movement position.</param>
        /// <returns>The attempted Move.</returns>
        public override Move CanMovePiece(int x, char y) {
            Move attemptedMove = Move.Blank;

            if (this.IsTakingPiece) {
                attemptedMove = canMoveWithRule(x, y, TAKE_PIECE_RULE);
            } else if(this.IsQueen) {
                attemptedMove = canMoveWithRule(x, y, QUEENS_RULE);
            } else if(this.IsFirstMove) {
                attemptedMove = canMoveWithRule(x, y, FIRST_MOVE_RULE);
            } else { 
                attemptedMove = base.CanMovePiece(x, y);
            }

            if (!attemptedMove.IsBlank()) {
                if (this.IsQueen) {
                    var lateralMoves = new[] { CardinalDirection.East, CardinalDirection.North, CardinalDirection.West, CardinalDirection.South };
                    var diagonalMoves = new[] { CardinalDirection.NorthEast, CardinalDirection.NorthWest, CardinalDirection.SouthEast, CardinalDirection.SouthWest };

                    if (lateralMoves.Contains(attemptedMove.Direction)) {
                        if (this.Parent.Pieces.Any(p => this.IsNotSame(p) && p.X.IsBetween(this.X, x, !this.IsTakingPiece) && p.Y.IsBetween(this.Y, y, !this.IsTakingPiece))) {
                            attemptedMove = Move.Blank;
                        }
                    } else if (diagonalMoves.Contains(attemptedMove.Direction)) {
                        if (this.Parent.Pieces.Any(p => this.IsNotSame(p) && p.X == p.Y && p.X.IsBetween(this.X, x, !this.IsTakingPiece) && p.Y.IsBetween(this.Y, y, !this.IsTakingPiece))) {
                            attemptedMove = Move.Blank;
                        }
                    }
                } else if (!this.IsTakingPiece) {
                    if (this.Parent.Pieces.Any(p => this.IsNotSame(p) && p.Y.IsBetween(this.Y, y) && p.X == x)) {
                        attemptedMove = Move.Blank;
                    }
                }
            }

            if (this.SetFlags) {
                if (this.Team == Team.White && y == 'A') {
                    this.IsQueen = true;
                } else if (this.Team == Team.Black && y == 'H') {
                    this.IsQueen = true;
                }

                if (!attemptedMove.IsBlank()) {
                    this.IsFirstMove = false;
                }
            }

            return attemptedMove;
        }
    }

    /// <summary>
    /// Chess.Pieces.Rook
    /// Represents the Rook on the chess board.
    /// </summary>
    class Rook : Piece {
        /// <summary>
        /// Returns the name of the Rook instance identified by the letter R and the first letter of the current Team.
        /// </summary>
        public override string Name {
            get {
                return $"R{Team.ToString()[0]}";
            }
        }

        /// <summary>
        /// Creates a new instance of the Rook class.
        /// </summary>
        /// <param name="tile">The location of the Rook on the chess board.</param>
        /// <param name="team">The team that the Rook is on.</param>
        /// <param name="parent">The chess board to allow referencing other Piece objects.</param>
        public Rook(string tile, Team team, Board parent) : base(tile, team, parent) {
            MovementRules.Add(STANDARD_RULE, r => (r.Horizonal > 0 && r.Vertical == 0) || (r.Horizonal == 0 && r.Vertical > 0));
        }

        /// <summary>
        /// Determines whether the Rook instance can move under any special cirumstances.
        /// </summary>
        /// <param name="x">Horizontal movement position.</param>
        /// <param name="y">Vertical movement position.</param>
        /// <returns>The attempted Move.</returns>
        public override Move CanMovePiece(int x, char y) {
            var attemptedMove = base.CanMovePiece(x, y);

            if (!attemptedMove.IsBlank()) {
                if (this.Parent.Pieces.Any(p => this.IsNotSame(p) && p.X.IsBetween(this.X, x, !this.IsTakingPiece) & p.Y.IsBetween(this.Y, y, !this.IsTakingPiece))) {
                    attemptedMove = Move.Blank;
                }
            }

            return attemptedMove;
        }
    }

    /// <summary>
    /// Chess.Pieces.Knight
    /// Represents the Knight on the chess board.
    /// </summary>
    class Knight : Piece {
        /// <summary>
        /// Returns the name of the Knight instance identified by the letter N and the first letter of the current Team.
        /// </summary>
        public override string Name {
            get {
                return $"N{Team.ToString()[0]}";
            }
        }

        /// <summary>
        /// Creates a new instance of the Knight class.
        /// </summary>
        /// <param name="tile">The location of the Knight on the chess board.</param>
        /// <param name="team">The team that the Knight is on.</param>
        /// <param name="parent">The chess board to allow referencing other Piece objects.</param>
        public Knight(string tile, Team team, Board parent) : base(tile, team, parent) {
            MovementRules.Add(STANDARD_RULE, r => (r.Horizonal == 1 && (r.Vertical == r.Horizonal * 2)) || (r.Vertical == 1 && (r.Horizonal == r.Vertical * 2)));
        }

        /// <summary>
        /// Determines whether the Knight instance can move under any special cirumstances.
        /// </summary>
        /// <param name="x">Horizontal movement position.</param>
        /// <param name="y">Vertical movement position.</param>
        /// <returns>The attempted Move.</returns>
        public override Move CanMovePiece(int x, char y) {
            var attemptedMove = base.CanMovePiece(x, y);

            if (!attemptedMove.IsBlank()) {
                if (!this.IsTakingPiece) {
                    if (this.Parent.Pieces.Any(p => this.IsNotSame(p) && p.X == x && p.Y == y)) {
                        attemptedMove = Move.Blank;
                    }
                }
            }

            return attemptedMove;
        }
    }

    /// <summary>
    /// Chess.Pieces.Bishop
    /// Represents the Bishop on the chess board.
    /// </summary>
    class Bishop : Piece {
        /// <summary>
        /// Returns the name of the Bishop instance identified by the letter B and the first letter of the current Team.
        /// </summary>
        public override string Name {
            get {
                return $"B{Team.ToString()[0]}";
            }
        }

        /// <summary>
        /// Creates a new instance of the Bishop class.
        /// </summary>
        /// <param name="tile">The location of the Bishop on the chess board.</param>
        /// <param name="team">The team that the Bishop is on.</param>
        /// <param name="parent">The chess board to allow referencing other Piece objects.</param>
        public Bishop(string tile, Team team, Board parent) : base(tile, team, parent) {
            MovementRules.Add(STANDARD_RULE, r => r.Horizonal == r.Vertical);
        }

        /// <summary>
        /// Determines whether the Bishop instance can move under any special cirumstances.
        /// </summary>
        /// <param name="x">Horizontal movement position.</param>
        /// <param name="y">Vertical movement position.</param>
        /// <returns>The attempted Move.</returns>
        public override Move CanMovePiece(int x, char y) {
            var attemptedMove = base.CanMovePiece(x, y);

            if (!attemptedMove.IsBlank()) {
                if (this.Parent.Pieces.Any(p => this.IsNotSame(p) && p.X == p.Y && p.X.IsBetween(this.X, x, !this.IsTakingPiece) && p.Y.IsBetween(this.Y, y, !this.IsTakingPiece))) {
                    attemptedMove = Move.Blank;
                }
            }

            return attemptedMove;
        }
    }

    /// <summary>
    /// Chess.Pieces.Queen
    /// Represents the Queen on the chess board.
    /// </summary>
    class Queen : Piece {
        /// <summary>
        /// Returns the name of the Queen instance identified by the letter Q and the first letter of the current Team.
        /// </summary>
        public override string Name {
            get {
                return $"Q{Team.ToString()[0]}";
            }
        }

        /// <summary>
        /// Creates a new instance of the Queen class.
        /// </summary>
        /// <param name="tile">The location of the Queen on the chess board.</param>
        /// <param name="team">The team that the Queen is on.</param>
        /// <param name="parent">The chess board to allow referencing other Piece objects.</param>
        public Queen(string tile, Team team, Board parent) : base(tile, team, parent) {
            MovementRules.Add(STANDARD_RULE, r => (r.Vertical == r.Horizonal) || (r.Horizonal > 0 && r.Vertical == 0) || (r.Horizonal == 0 && r.Vertical > 0));
        }

        /// <summary>
        /// Determines whether the Queen instance can move under any special cirumstances.
        /// </summary>
        /// <param name="x">Horizontal movement position.</param>
        /// <param name="y">Vertical movement position.</param>
        /// <returns>The attempted Move.</returns>
        public override Move CanMovePiece(int x, char y) {
            var attemptedMove = base.CanMovePiece(x, y);

            if (!attemptedMove.IsBlank()) {
                var lateralMoves = new[] { CardinalDirection.East, CardinalDirection.North, CardinalDirection.West, CardinalDirection.South };
                var diagonalMoves = new[] { CardinalDirection.NorthEast, CardinalDirection.NorthWest, CardinalDirection.SouthEast, CardinalDirection.SouthWest };

                if (lateralMoves.Contains(attemptedMove.Direction)) {
                    if (this.Parent.Pieces.Any(p => this.IsNotSame(p) && p.X.IsBetween(this.X, x, !this.IsTakingPiece) && p.Y.IsBetween(this.Y, y, !this.IsTakingPiece))) {
                        attemptedMove = Move.Blank;
                    }
                } else if (diagonalMoves.Contains(attemptedMove.Direction)) {
                    if (this.Parent.Pieces.Any(p => this.IsNotSame(p) && p.X == p.Y && p.X.IsBetween(this.X, x, !this.IsTakingPiece) && p.Y.IsBetween(this.Y, y, !this.IsTakingPiece))) {
                        attemptedMove = Move.Blank;
                    }
                }
            }

            return attemptedMove;
        }
    }

    /// <summary>
    /// Chess.Pieces.King
    /// Represents the King on the chess board.
    /// </summary>
    class King : Piece {
        /// <summary>
        /// Returns the name of the King instance identified by the letter K and the first letter of the current Team.
        /// </summary>
        public override string Name {
            get {
                return $"K{Team.ToString()[0]}";
            }
        }

        public IEnumerable<Tile> NeighboringSpots {
            get {
                var validTiles = new List<string>();

                validTiles.Add($"{this.X + 1}{this.Y + 1}");
                validTiles.Add($"{this.X}{this.Y + 1}");
                validTiles.Add($"{this.X - 1}{this.Y + 1}");
                validTiles.Add($"{this.X + 1}{this.Y}");
                validTiles.Add($"{this.X - 1}{this.Y}");
                validTiles.Add($"{this.X + 1}{this.Y - 1}");
                validTiles.Add($"{this.X}{this.Y - 1}");
                validTiles.Add($"{this.X - 1}{this.Y - 1}");

                return this.Parent.Tiles.Where(t => validTiles.Contains(t.Location));
            }
        }

        /// <summary>
        /// Determines whether or not the King instance needs to escape or be defended this turn.
        /// </summary>
        public bool IsInCheck { get; set; }

        /// <summary>
        /// Determines whether or not anything can be done to remove the King instance from check.
        /// </summary>
        public bool InCheckMate { get; set; }

        /// <summary>
        /// Creates a new instance of the King class.
        /// </summary>
        /// <param name="tile">The location of the King on the chess board.</param>
        /// <param name="team">The team that the King is on.</param>
        /// <param name="parent">The chess board to allow referencing other Piece objects.</param>
        public King(string tile, Team team, Board parent) : base(tile, team, parent) {
            MovementRules.Add(STANDARD_RULE, r => r.Vertical == 1 || r.Horizonal == 1);
        }

        /// <summary>
        /// Determines whether the King instance can move under any special cirumstances.
        /// </summary>
        /// <param name="x">Horizontal movement position.</param>
        /// <param name="y">Vertical movement position.</param>
        /// <returns>The attempted Move.</returns>
        public override Move CanMovePiece(int x, char y) {
            var attemptedMove = base.CanMovePiece(x, y);

            if (!attemptedMove.IsBlank()) {
                if (this.Parent.Pieces.Any(p => this.IsNotSame(p) && p.X == x && p.Y == y)) {
                    attemptedMove = Move.Blank;
                } else {
                    foreach (var pce in this.Parent.Pieces.Where(p => p != null && p.Team != this.Team && typeof(King) != p.GetType())) {
                        pce.SetFlags = false;

                        if (!pce.CanMovePiece(this.X, this.Y).IsBlank()) {
                            attemptedMove = Move.Blank;
                        }

                        pce.SetFlags = true;
                    }

                }
            }

            return attemptedMove;
        }
    }
}