using Chess.Pieces;

namespace Chess.Moves {
    /// <summary>
    /// Chess.Moves.Move
    /// Value type object to represent moves taken by a piece during a game.
    /// </summary>
    struct Move {
        /// <summary>
        /// A readonly Move object to represent a stasis.
        /// </summary>
        public static readonly Move Blank;

        /// <summary>
        /// The vertical distance moved.
        /// </summary>
        public int Vertical { get; private set; }

        /// <summary>
        /// The horizontal distance moved.
        /// </summary>
        public int Horizonal { get; private set; }

        /// <summary>
        /// The direction in which the move was taken.
        /// </summary>
        public CardinalDirection Direction { get; private set; }

        public Piece MovingPiece { get; private set; }

        /// <summary>
        /// Creates a new instance of the Move object.
        /// </summary>
        /// <param name="h">The horizontal distance of the move.</param>
        /// <param name="v">The vertical distance of the move.</param>
        /// <param name="direction">The direction in which the move is going.</param>
        /// <param name="movingPiece">The piece performing the move.</param>
        public Move(int h, int v, CardinalDirection direction, Piece movingPiece) {
            Horizonal = h;
            Vertical = v;
            Direction = direction;
            MovingPiece = movingPiece;
        }
        
        /// <summary>
        /// Determines if the current instance of the move object represents a static move.
        /// </summary>
        /// <returns>True if the move induces no change.</returns>
        public bool IsBlank() {
            return Blank.Equals(this);
        }
    }
    
    /// <summary>
    /// Chess.Moves.CardinalDirection
    /// enum to represent applicable directions that a Move object can go.
    /// </summary>
    enum CardinalDirection {
        None = 0,
        North = 1,
        South = 2,
        East = 3,
        West = 4,
        NorthWest = 5,
        NorthEast = 6,
        SouthWest = 7,
        SouthEast = 8
    }
}
