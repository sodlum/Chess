using System;

namespace Chess.Exceptions {
    /// <summary>
    /// Chess.Exceptions.MaxTileCapacityExceededException
    /// Thrown when a Tile object is added after the max number of tiles has been reached.
    /// </summary>
    class MaxTileCapacityExceededException : Exception { }

    /// <summary>
    /// Chess.Exceptions.MaxPieceCapacityExceededException
    /// Thrown when a Piece object is added after the max number of pieces has been reached.
    /// </summary>
    class MaxPieceCapacityExceededException : Exception { }

    /// <summary>
    /// Chess.Exceptions.MaxPieceCapacityExceededException
    /// Throw when an invalid location is provided when attempting to reference a location on the chess board.
    /// </summary>
    class InvalidTileException : Exception {
        /// <summary>
        /// Creates a new instance of the InvalidTileException class with the provided <paramref name="Message"/>.
        /// </summary>
        /// <param name="Message">The message to be displayed by the excepiton.</param>
        public InvalidTileException(string Message) : base(Message) { }
    }
}
