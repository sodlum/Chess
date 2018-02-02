using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chess.Tiles;
using Chess.Pieces;
using Chess.Moves;
using Chess.Exceptions;
using System;

namespace Chess.Collections {
    /// <summary>
    /// Chess.Collections.PieceCollection
    /// Specialized piece collection to represent chess pieces.
    /// </summary>
    class PieceCollection : IEnumerable<Piece> {
        private const int _space = 32;
        private Piece[] _pieces;

        /// <summary>
        /// Gets the piece at the corresponding location.
        /// </summary>
        /// <param name="y">Vertical location.</param>
        /// <param name="x">Horizontal location.</param>
        /// <returns>Piece at corresponding location based on <paramref name="y"/><paramref name="x"/>.</returns>
        public Piece this[char y, int x] {
            get {
                var piece = _pieces.SingleOrDefault(p => p != null && p.X == x && p.Y == y);
                return piece;
            }
        }

        /// <summary>
        /// Tracks the total number of pieces currently in the collection.
        /// </summary>
        public int TotalPieces { get; private set; }

        /// <summary>
        /// Creates a new instance of the PieceCollection class.
        /// Has a predefined size of 32 available memory blocks.
        /// </summary>
        public PieceCollection() {
            _pieces = new Piece[_space];
        }

        /// <summary>
        /// Adds a new Piece object to the instance of the PieceCollection class.
        /// </summary>
        /// <param name="piece">The piece being added.</param>
        /// <returns>True if the addition was successful. Throws an exception otherwise.</returns>
        public bool AddPiece(Piece piece) {
            if(this.TotalPieces < _space) {
                this._pieces[this.TotalPieces] = piece;
                this.TotalPieces++;
            } else {
                throw new MaxPieceCapacityExceededException();
            }

            return true;
        }

        /// <summary>
        /// Removes given piece from the instance of the PieceCollection class
        /// </summary>
        /// <param name="piece">The piece being removed.</param>
        /// <returns>True if the removal was successful. False if the piece does not exist.</returns>
        public bool RemovePiece(Piece piece) {
            var i = 0;

            while (i <= this.TotalPieces) {
                var p = this._pieces[i];

                if (p != null) {
                    if (p.ID == piece.ID) {
                        break;
                    }
                }

                i++;
            }

            this._pieces[i] = null;
            this.TotalPieces--;

            return true;
        }
        
        /// <summary>
        /// Iterates through the Piece objects in the instance of the PieceCollection class.
        /// </summary>
        /// <returns>Current Piece object in the iteration.</returns>
        public IEnumerator<Piece> GetEnumerator() {
            foreach(var piece in _pieces) {
                yield return piece;
            }
        }

        /// <summary>
        /// GetEnumerator implementation.
        /// </summary>
        /// <returns>Iterator for the current PieceCollection instance.</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return _pieces.GetEnumerator();
        }
    }

    /// <summary>
    /// Chess.Collections.TileCollection
    /// Specialized collection to represent tiles on the chess board.
    /// </summary>
    class TileCollection : IEnumerable<Tile> {
        private const int _space = 64;
        private Tile[] _tiles;

        /// <summary>
        /// Tracks the total number of tiles in the current instance of the TileCollection class.
        /// </summary>
        public int TotalTiles { get; private set; }

        /// <summary>
        /// Gets the tile at the given location represented by <paramref name="l"/>.
        /// </summary>
        /// <param name="l">Location of the tile to be returned.</param>
        /// <returns>Tile at the location represented by l. Returns null otherwise.</returns>
        public Tile this[string l] {
            get {
                return this._tiles.SingleOrDefault(t => t.Location == l);
            }
        }

        /// <summary>
        /// Creates a new instance of the TileCollection class.
        /// Has a predefined size of 64 available memory blocks.
        /// </summary>
        public TileCollection() {
            this._tiles = new Tile[_space];
        }

        /// <summary>
        /// Adds a new Tile object to the current instance of the TileCollection class
        /// </summary>
        /// <param name="tile">The Tile being added.</param>
        /// <returns>True if the tile was successfully added. Throws an exception otherwise.</returns>
        public bool AddTile(Tile tile) {
            if(this.TotalTiles < _space) {
                this._tiles[this.TotalTiles] = tile;
                this.TotalTiles++;
            } else {
                throw new MaxTileCapacityExceededException();
            }

            return true;
        }

        /// <summary>
        /// Iterates through the Tile objects in the instance of the TileCollection class.
        /// </summary>
        /// <returns>Current Tile object in the iteration.</returns>
        public IEnumerator<Tile> GetEnumerator() {
            foreach(Tile tile in this._tiles) {
                yield return tile;
            }
        }

        /// <summary>
        /// GetEnumerator implementation.
        /// </summary>
        /// <returns>Iterator for the current TileCollection instance.</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return _tiles.GetEnumerator();
        }
    }

    /// <summary>
    /// Chess.Collections.MoveCollection
    /// Specialized collection to represent moves taken by individual Piece objects.
    /// </summary>
    class MoveCollection : IEnumerable<Move> {
        private int _size;
        private int _capacity;
        private Move[] _moves;

        public int TotalMoves { get; private set; }

        /// <summary>
        /// Gets the #<paramref name="i"/> move.
        /// </summary>
        /// <param name="i">The specific move taken by the current Piece.</param>
        /// <returns>The move identified by <paramref name="i"/></returns>
        public Move this[int i] {
            get {
                return _moves[i];
            }
        }

        /// <summary>
        /// Creates a new instance of the MoveCollection class with 10 allocated memory blocks.
        /// </summary>
        public MoveCollection() {
            _capacity = 10;
            _moves = new Move[_capacity];
        }

        /// <summary>
        /// Creates a new instance of the MoveCollection class with a specific <paramref name="capacity"/>.
        /// </summary>
        /// <param name="capacity">The size of the instance of the MoveCollection class.</param>
        public MoveCollection(int capacity) {
            _capacity = capacity;
            _moves = new Move[capacity];
        }

        /// <summary>
        /// Creates a new instance of the MoveCollection class based on an enumerable collection of Move objects.
        /// </summary>
        /// <param name="moves">Enumerable collection of Move objects.</param>
        public MoveCollection(IEnumerable<Move> moves) {
            _size = moves.Count();
            _capacity = _size * 2;
            _moves = new Move[_capacity];

            for (var i = 0; i < _size; i++) {
                _moves[i] = moves.ElementAt(i);
            }
        }

        /// <summary>
        /// Doubles current capacity to prevent stack overflow.
        /// </summary>
        private void increaseCapacity() {
            var moves = _moves;
            _moves = new Move[_capacity * 2];
            _capacity *= 2;

            for(var i = 0; i < _size; i++) {
                _moves[i] = moves[i];
            }
        }

        /// <summary>
        /// Adds move to the current instance of the MoveCollection class.
        /// </summary>
        /// <param name="move">The Move object being added.</param>
        /// <returns>True if the Move object was successfully added.</returns>
        public bool AddMove(Move move) {
            if (this._size == this._capacity) {
                increaseCapacity();
            }

            _moves[this._size] = move;

            this.TotalMoves++;

            return true;
        }

        /// <summary>
        /// Iterates through the Move objects in the instance of the MoveCollection class.
        /// </summary>
        /// <returns>Current Move object in the iteration.</returns>
        public IEnumerator<Move> GetEnumerator() {
            foreach (var move in _moves) {
                yield return move;
            }
        }

        /// <summary>
        /// GetEnumerator implementation.
        /// </summary>
        /// <returns>Iterator for the current MoveCollection instance.</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return _moves.GetEnumerator();
        }
    }
}