using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChessGame
{
    [Serializable]

    public abstract class Piece
    {
        [field: SerializeField] public PieceType Type { get; private set; }
        [field: SerializeField] public PieceColor PieceColor { get; private set; }
        [field: SerializeField] public int MaterialCost { get; private set; }
        [field: SerializeField] public Vector2Int Coordinate { get; protected set; }

        public Vector2Int PreviousCoordinate { get; protected set; }

        public event Action<Vector2Int> onPositionChanged;
        public event Action<PieceType> onTypeChanged;
        public event Action<Piece> onPieceTaken;

        public Board Board { get; set; }

        public bool HasMoved { get; protected set; }

        public bool HasCheckOnKing { get; set; }

        protected Piece(PieceType type, Vector2Int coordinate, PieceColor pieceColor, Board board = null)
        {
            Type = type;
            Coordinate = coordinate;
            MaterialCost = GameConstants.MaterialCostsDictionary[type];
            PieceColor = pieceColor;
            Board = board;

            HasMoved = false;

        }

        protected void OnPositionChanged(Vector2Int pos)
        {
            onPositionChanged?.Invoke(pos);
        }

        public void Take()
        {
            onPieceTaken?.Invoke(this);
        }


        public virtual void SetPositionOnBoard(Vector2Int position, bool isIntialSetup, bool bypassTurnOrder)
        {
            
        }

        protected void CompleteMove(Piece piece, Vector2Int position)
        {
            // set position and invoke event
            Coordinate = position;
            onPositionChanged?.Invoke(position);
            HasMoved = true;

            King king = Board.GetKing(this);
            King opponentsKing = Board.GetOpponentsKing(this);

            king.IsInCheck = Board.GetAttackingPieces(king).Count > 0;
            opponentsKing.IsInCheck = Board.GetAttackingPieces(opponentsKing).Count > 0;

            GameController.Instance.SetLastMovedPiece(this);

            GameController.Instance.CurrentTurn = GameController.Instance.CurrentTurn == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;
        }
        
        /// <summary>
        /// Returns the available moves this piece has
        /// </summary>
        /// <param name="board">The board in play</param>
        /// <returns></returns>
        public abstract HashSet<Move> GetLegalMoves();



    }
}