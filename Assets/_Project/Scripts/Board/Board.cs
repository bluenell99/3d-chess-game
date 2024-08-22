using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace ChessGame
{
    public class Board
    {

        #region SETUP

        private HashSet<Vector2Int> Squares { get; }
        private HashSet<Piece> PiecesInPlay { get; set; }

        public Piece LastPieceMoved { get; set; }
        private int BoardSize { get; set; }

        /// <summary>
        /// Create a new board with a given size
        /// </summary>
        /// <param name="boardSize">The square count in each direction (Standard Chess board is 8x8)</param>
        public Board(int boardSize)
        {
            BoardSize = boardSize;
            Squares = new HashSet<Vector2Int>();
            PiecesInPlay = new HashSet<Piece>();

            CalculateSquareCoordinates();

        }

        /// <summary>
        /// Calculates the coordinates for each grid on the board
        /// </summary>
        private void CalculateSquareCoordinates()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    Vector2Int current = new Vector2Int(i, j);
                    Squares.Add(current);
                }
            }
        }

        #endregion

        # region ACCESS

        /// <summary>
        /// Returns all the opponents pieces
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public HashSet<Piece> GetOpponentPieces(Piece piece)
        {
            var pieces = new HashSet<Piece>();

            foreach (var p in PiecesInPlay.Where(p => p.PieceColor != piece.PieceColor))
            {
                pieces.Add(p);
            }

            return pieces;
        }

        /// <summary>
        /// Returns all of our pieces
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public HashSet<Piece> GetPieces(Piece piece)
        {
            return PiecesInPlay.Where(p => p.PieceColor == piece.PieceColor).ToHashSet();
        }



        /// <summary>
        /// Adds a piece to play
        /// </summary>
        /// <param name="piece"></param>
        public void AddPiece(Piece piece)
        {
            PiecesInPlay.Add(piece);
        }

        /// <summary>
        /// Removes a piece from play
        /// </summary>
        /// <param name="piece"></param>
        public void RemovePiece(Piece piece)
        {
            if (!PiecesInPlay.Contains(piece))
            {
                Debug.Log("Piece is not in play");
            }

            PiecesInPlay.Remove(piece);
        }

        /// <summary>
        /// Is a given square on the board
        /// </summary>
        /// <param name="square"></param>
        /// <returns></returns>
        public bool IsSquareOnBoard(Vector2Int square) => Squares.Contains(square);

        /// <summary>
        /// Checks if a square on the board is occupied by another piece
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public bool IsSquareOccupied(Vector2Int coordinate)
        {
            return PiecesInPlay.Any(p => p.Coordinate == coordinate);
        }

        /// <summary>
        /// Try and get a piece from the board
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="piece"></param>
        /// <returns></returns>
        public bool TryGetPieceFromSquare(Vector2Int coordinate, out Piece piece)
        {
            foreach (var p in PiecesInPlay.Where(p => p.Coordinate == coordinate))
            {
                piece = p;
                return true;
            }

            piece = null;
            return false;
        }

        /// <summary>
        /// Returns this pieces king
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public King GetKing(Piece piece)
        {
            return (King)GetPieces(piece).First(p => p is King);
        }

        /// <summary>
        /// Returns the opponent of this piece's king
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public King GetOpponentsKing(Piece piece)
        {
            return (King)GetOpponentPieces(piece).First(p => p is King);
        }

        public HashSet<Piece> GetAttackingPieces(King king)
        {
            HashSet<Piece> attackingPieces = new();
            var opponentPieces = GetOpponentPieces(king).Where(p => p is not King);

            foreach (var opponentPiece in opponentPieces)
            {
                var possibleMoves = opponentPiece.GetLegalMoves();

                foreach (var move in possibleMoves)
                {
                    if (move.Coordinate == king.Coordinate)
                    {
                        attackingPieces.Add(opponentPiece);
                    }
                }
            }

            return attackingPieces;
        }

        public HashSet<Move> EvaluateMoveLegality(HashSet<Move> possibleMoves, Piece piece)
        {
            King king = GetKing(piece);

            if (!king.IsInCheck)
                return possibleMoves;

            Debug.Log("King is in check");

            Piece checkingPiece = GetSingleCheckingPiece(piece);

            if (checkingPiece == null)
                return new HashSet<Move>();

            Debug.Log($"Checking piece {checkingPiece.PieceColor}:{checkingPiece.Type} at {checkingPiece.Coordinate}");

            HashSet<Move> checkingPieceMoves =
                checkingPiece.GetLegalMoves().Where(m => m.IsDeliveringCheck).ToHashSet();

            Debug.Log(
                $"{king.PieceColor} king at {king.Coordinate} is under check from {checkingPiece.PieceColor}: {checkingPiece.Type} at {checkingPiece.Coordinate}");

            return GetBlockingOrCapturingMoves(possibleMoves, checkingPiece, checkingPieceMoves);

        }

        public HashSet<Vector2Int> GetCastlingSquares(King king, bool isKingSide)
        {
            HashSet<Vector2Int> requiredSquares = new();
            int rank = king.Coordinate.y;

            if (isKingSide)
            {
                requiredSquares.Add(new Vector2Int(5, rank));
                requiredSquares.Add(new Vector2Int(6, rank));
            }
            else
            {
                requiredSquares.Add(new Vector2Int(1, rank)); // b1 or b8
                requiredSquares.Add(new Vector2Int(2, rank)); // c1 or c8
                requiredSquares.Add(new Vector2Int(3, rank)); // d1 or d8
            }

            return requiredSquares;
        }

        #endregion

        # region INTERNALS

        private Piece GetSingleCheckingPiece(Piece piece)
        {
            HashSet<Piece> opponentPieces =
                GetOpponentPieces(piece).Where(p => p is not King && p.HasCheckOnKing).ToHashSet();

            if (opponentPieces.Count != 1) return null;

            Debug.Log(opponentPieces.First());
            return opponentPieces.First();

        }


        /// <summary>
        /// Gets all of this pieces moves that will block or capture a checking piece
        /// </summary>
        /// <param name="possibleMoves"></param>
        /// <param name="checkingPieces"></param>
        /// <param name="checkingMoves"></param>
        /// <returns></returns>
        private HashSet<Move> GetBlockingOrCapturingMoves(HashSet<Move> possibleMoves, Piece checkingPiece,
            HashSet<Move> checkingMoves)
        {
            HashSet<Move> legalMoves = new HashSet<Move>();

            foreach (var move in possibleMoves)
            {
                if (move.Coordinate == checkingPiece.Coordinate)
                {
                    legalMoves.Add(move);
                    continue;
                }

                if (checkingMoves.Any(m => m.Coordinate == move.Coordinate))
                {
                    legalMoves.Add(move);
                }
            }

            return legalMoves;
        }

        #endregion

        public void OnBoardStateUpdated()
        {
            
        }
    }
}