using Chess.Pieces;

namespace Chess.Tiles {
    /// <summary>
    /// Chess.Tiles.Tile
    /// Object to represent the area on a board in which a piece will occupy.
    /// </summary>
    class Tile {
        /// <summary>
        /// The geographical location of the Tile instance on the chess board.
        /// </summary>
        public string Location { get; private set; }

        /// <summary>
        /// The piece currently sharing the location of the Tile instance.
        /// </summary>
        public Piece CurrentOccupant { get; set; }

        /// <summary>
        /// Creates a new instance of the Tile class at the location represented by <paramref name="l"/>.
        /// </summary>
        /// <param name="l">The location of the Tile object.</param>
        public Tile(string l) {
            Location = l;
        }
    }
}