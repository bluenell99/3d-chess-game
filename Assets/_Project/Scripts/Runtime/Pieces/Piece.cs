using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    [Serializable]

    public abstract class Piece
    {
        
        /// <summary>
        /// The type of Piece this is
        /// </summary>
        public PieceType Type { get; private set; }
        /// <summary>
        /// The Piece's team colour
        /// </summary>
        public PieceColor PieceColor { get; private set; }
        
        /// <summary>
        /// The material cost of this Piece
        /// </summary>
        public int MaterialCost { get; private set; }
        
        /// <summary>
        /// This Piece's current coordinate on the Board
        /// </summary>
        public Vector2Int Coordinate { get;  set; }

        /// <summary>
        /// This Piece's last coordinate on the Board
        /// </summary>
        public Vector2Int PreviousCoordinate { get; protected set; }
        
        /// <summary>
        /// This Piece's starting coordinate on the Board
        /// </summary>
        public Vector2Int StartingCoordinate { get; protected set; }

        /// <summary>
        /// Invoked when the Piece's position has changed
        /// </summary>
        public event Action<Vector2Int> onPositionChanged;
        /// <summary>
        /// Invoked when this Piece is captured
        /// </summary>
        public event Action<Piece> onPieceTaken;
        /// <summary>
        /// Invoked when this Piece has completed it's move
        /// </summary>
        public event Action<Piece> onPieceTurnEnd;

        /// <summary>
        /// Reference to the Board
        /// </summary>
        public Board Board { get; set; }

        /// <summary>
        /// Has this Piece moved yet
        /// </summary>
        public bool HasMoved { get; protected set; }

        /// <summary>
        /// Does this Piece have check against the opponents King
        /// </summary>
        public bool HasCheckOnKing { get; set; }

        /// <summary>
        /// Create a new Piece
        /// </summary>
        /// <param name="type"></param>
        /// <param name="coordinate"></param>
        /// <param name="pieceColor"></param>
        /// <param name="board"></param>
        protected Piece(PieceType type, Vector2Int coordinate, PieceColor pieceColor, Board board = null)
        {
            Type = type;
            Coordinate = coordinate;
            StartingCoordinate = coordinate;
            MaterialCost = GameConstants.MaterialCostsDictionary[type];
            PieceColor = pieceColor;
            Board = board;

            HasMoved = false;

        }

        /// <summary>
        /// Triggers the onPositionChanged event
        /// </summary>
        /// <param name="pos"></param>
        protected void OnPositionChanged(Vector2Int pos)
        {
            onPositionChanged?.Invoke(pos);
        }

        /// <summary>
        /// Triggers the onPieceTaken event
        /// </summary>
        public void Take()
        {
            onPieceTaken?.Invoke(this);
        }

        /// <summary>
        /// Updates the Piece's position on the Board
        /// </summary>
        /// <param name="position">The new position</param>
        /// <param name="isInitialSetup">Is this called from the Board intialisation</param>
        /// <param name="bypassTurnOrder">Does this bypass the turn order system</param>
        public abstract void SetPositionOnBoard(Vector2Int position, bool isInitialSetup, bool bypassTurnOrder);

        /// <summary>
        /// Finalises a Move
        /// </summary>
        /// <param name="position"></param>
        protected void CompleteMove(Vector2Int position)
        {
            // set position and invoke event
            Coordinate = position;
            HasMoved = true;
            
            // Notify the board that this is the last piece moved
            Board.SetLastMovedPiece(this);

            // increment the full move clock after blacks move
            if (PieceColor == PieceColor.Black)
                Board.IncrementFullMoveClock();;
            
            Board.IncrementHalfMoveClock();;
            
            // Invoke the end turn event
            onPositionChanged?.Invoke(position);
            onPieceTurnEnd?.Invoke(this);
            
        }
        
        /// <summary>
        /// Returns the legal moves this piece has
        /// </summary>
        public abstract HashSet<Move> GetLegalMoves();


        /// <summary>
        /// Set's the Piece back to it's initial starting position
        /// </summary>
        public void ResetPiece()
        {
            SetPositionOnBoard(StartingCoordinate, true, false);
            HasMoved = false;
        }

    }
}