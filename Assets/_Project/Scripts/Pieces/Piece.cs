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
        [field: SerializeField] public Vector2Int Coordinate { get; private set; }

        public Vector2Int PreviousCoordinate { get; private set; }

        public event Action<Vector2Int> onPositionChanged;
        public event Action<PieceType> onTypeChanged;
        public event Action<Piece> onPieceTaken;

        public Board Board { get; set; }

        public bool HasMoved { get; private set; }

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


        public virtual void SetPositionOnBoard(Vector2Int position, bool isIntialSetup, bool bypassTurnOrder)
        {
            PreviousCoordinate = Coordinate;

            if (isIntialSetup)
            {
                Coordinate = position;
                onPositionChanged?.Invoke(position);
                return;
            }

            // checks if this is an En-Passant capture
            if (this is Pawn && GameController.Instance.LastPieceMoved is Pawn lastMovedPawn)
            {
                int direciton = PieceColor == PieceColor.White ? 1 : -1;
                Vector2Int enpassantTarget = new Vector2Int(position.x, position.y - direciton);


                if (lastMovedPawn.Coordinate == enpassantTarget && Math.Abs(PreviousCoordinate.y - position.y) == 1 && PreviousCoordinate.x != position.x)
                {
                    // Remove the captured pawn from the board
                    lastMovedPawn.onPieceTaken?.Invoke(lastMovedPawn);
                }
            }

            // Check if square is occupied
            if (Board.TryGetPieceFromSquare(position, out Piece occupyingPiece))
            {
                if (occupyingPiece.PieceColor != PieceColor)
                {
                    // invoke the taken event on the opponents Piece
                    occupyingPiece.onPieceTaken?.Invoke(occupyingPiece);
                }
                else // this should only be called when castling opportunity
                {
                    if (occupyingPiece is King)
                    {
                        bool isKingside = Coordinate.x > occupyingPiece.Coordinate.x;

                        if (isKingside)
                        {
                            // setup move for rook (+1 as move technically is the kings position, so we need to go right one square of where the king would be
                            position = new Vector2Int(position.x + 1, position.y);

                            // trigger move for king (+2 takes the king right two squares
                            Vector2Int kingPosition = new Vector2Int(occupyingPiece.Coordinate.x + 2, occupyingPiece.Coordinate.y);
                            occupyingPiece.SetPositionOnBoard(kingPosition, false, true); // we bypass the turn order to ensure turn doesn't change after the king is moved

                        }
                        else
                        {
                            // setup move for rook (-1 as move technically is the kings position, so we need to go left one square of where the king would be
                            position = new Vector2Int(position.x - 1, position.y);

                            // trigger move for king (-2 takes the king left two squares
                            Vector2Int kingPosition = new Vector2Int(occupyingPiece.Coordinate.x - 2, occupyingPiece.Coordinate.y);
                            occupyingPiece.SetPositionOnBoard(kingPosition, false, true);
                        }
                    }
                }
            }

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