using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;


namespace ChessGame
{
    public class Board
    {

        #region SETUP

        /// <summary> The Squares on the board </summary>
        public  HashSet<Vector2Int> Squares { get; private set; }
        
        /// <summary> All the Pieces currently in play</summary> 
        public HashSet<Piece> PiecesInPlay { get; private set; }

        /// <summary> The last piece that moved </summary>
        public Piece LastPieceMoved { get; set; }
        
        /// <summary>
        /// The number of half-moves this board currently has
        /// </summary>
        /// <remarks> Halfmoves are reset when a Piece is captured, or a Pawn has moved</remarks>
        public int HalfMoveClock { get; private set; }
        
        /// <summary>
        /// The number of Fullmoves this board currently has
        /// </summary>
        /// <remarks>Fullmoves increment after both sides have moved. </remarks>
        public int FullMoveNumber { get; private set; }
        
        /// <summary>
        /// The total number of squares in one dimension
        /// </summary>
        private int BoardSize { get; set; }

        /// <summary>
        /// Invoked when the board is reset
        /// </summary>
        public event Action onBoardReset;

        /// <summary>
        /// Create a new board with a given size
        /// </summary>
        /// <param name="boardSize">The square count in each direction (Standard Chess board is 8x8)</param>
        public Board(int boardSize)
        {
            BoardSize = boardSize;
            Squares = new HashSet<Vector2Int>();
            PiecesInPlay = new HashSet<Piece>();

            HalfMoveClock = 0;
            FullMoveNumber = 1;

            CalculateSquareCoordinates();

        }

        /// <summary>
        /// Calculates the coordinates for each grid on the board
        /// </summary>
        private void CalculateSquareCoordinates()
        {
            // The Board cannot have a negative or zero size
            if (BoardSize <=0)
                throw new Exception("Board size must be a positive integer");
            
            // Iterate in each dimension to populate the Squares HashSet
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
        /// Is the Board currentlyt evaluating Check
        /// </summary>
        public bool IsEvaluatingCheck { get; set; } = false;
        
        /// <summary>
        /// Returns all the opponents pieces
        /// </summary>
        /// <param name="piece">Our Piece</param>
        /// <returns>HashSet of all opponents Piece's</returns>
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
        /// <param name="piece">Our piece</param>
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
            // ensure Piece isn't null
            if (piece == null)
                throw new NullReferenceException("Piece cannot be null");

            piece.onPieceTurnEnd += EvaluateCheck;
            piece.onPieceTaken += RemovePiece;

            if (piece is Pawn pawn)
            {
                pawn.onPawnPromotionAvailable += RemovePiece;
            }
            
            PiecesInPlay.Add(piece);
        }

        private void EvaluateCheck(Piece piece)
        {
            IsEvaluatingCheck = true;
            
            // get both Kings
            King king = GetKing(piece);
            King opponentsKing = GetOpponentsKing(piece);

            if (king == null)
            {
                IsEvaluatingCheck = false;
                return;
            }

            if (opponentsKing == null)
            {
                IsEvaluatingCheck = false;
                return;
            }

            // update the King's IsInCheck property based on this move
            king.IsInCheck = GetAttackingPieces(king).Count > 0;
            opponentsKing.IsInCheck = GetAttackingPieces(opponentsKing).Count > 0;
            bool kingInCheckmate = king.IsCheckMate;
            bool opponentInCheckmate = opponentsKing.IsCheckMate;

            IsEvaluatingCheck = false;
        }

        /// <summary>
        /// Removes a piece from play
        /// </summary>
        /// <param name="piece"></param>
        public void RemovePiece(Piece piece)
        {
            // ensure Piece isn't null
            if (piece == null)
                throw new NullReferenceException("Piece cannot be null");
            
            // ensure the Board actually contains this Piece
            if (!PiecesInPlay.Contains(piece))
                throw new Exception("Pieces in play does not contain given piece");

            HalfMoveClock = 0;
            
            PiecesInPlay.Remove(piece);
        }

        public void ResetHalfMoveClock()
        {
            HalfMoveClock = 0;
        }

        public void IncrementFullMoveClock()
        {
            FullMoveNumber++;
        }

        public void IncrementHalfMoveClock()
        {
            HalfMoveClock++;
        }

        /// <summary>
        /// Resets the Board state to it's starting layout
        /// </summary>
        public void ResetBoard()
        {
            // iterate through every Piece, and reset
            foreach (var piece in PiecesInPlay)
            {
                piece.ResetPiece();
            }

            HalfMoveClock = 0;
            FullMoveNumber = 1;
            
            // broadcast a reset
            onBoardReset?.Invoke();
            
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
        public bool IsSquareOccupied(Vector2Int coordinate) => PiecesInPlay.Any(p => p.Coordinate == coordinate);

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
        /// Get's a piece from a specific square
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public Piece GetPieceFromSquare(Vector2Int coordinate)
        {
            if (TryGetPieceFromSquare(coordinate, out Piece piece))
            {
                return piece;
            }

            return null;
        }

        /// <summary>
        /// Returns this pieces king
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public King GetKing(Piece piece)
        {
            try
            {
                King king = (King)GetPieces(piece).First(p => p is King);
                return king;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        
        /// <summary>
        /// Gets the king of a given colour
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public King GetKing(PieceColor color)
        {
            try
            {
                return (King)PiecesInPlay.First(p => p.PieceColor == color && p is King);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        /// <summary>
        /// Returns the opponent of this piece's king
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        public King GetOpponentsKing(Piece piece)
        {
            try
            {
                King king = (King)GetOpponentPieces(piece).First(p => p is King);
                return king;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        /// <summary>
        /// Gets all the opponent's Pieces that are directly attacking the King
        /// </summary>
        /// <param name="king">The king to check against</param>
        /// <returns>HashSet of Piece's</returns>
        /// <remarks>This doesn't return their King as part of the collection, as the King can never be in a position where it's under attack from another King</remarks>
        public HashSet<Piece> GetAttackingPieces(King king)
        {
            // Create new HashSet
            HashSet<Piece> attackingPieces = new();
            // Get all the opponent's Pieces, not including their King
            var opponentPieces = GetOpponentPieces(king).Where(p => p is not King);

            // iterate through all the opponent's Pieces
            foreach (var opponentPiece in opponentPieces)
            {
                // get their legal moves
                var possibleMoves = opponentPiece.GetLegalMoves();

                // iterate through their moves
                foreach (var move in possibleMoves)
                {
                    // if the move's target coordinate is the given King's coordinate, it's considered attacked, and we add it to the HashSet to return
                    if (move.Coordinate == king.Coordinate)
                    {
                        attackingPieces.Add(opponentPiece);
                    }
                }
            }

            return attackingPieces;
        }

        /// <summary>
        /// Evaluates the legality of a set of possible Moves
        /// </summary>
        /// <param name="possibleMoves"></param>
        /// <param name="piece"></param>
        /// <returns></returns>
        /// <remarks>If the Piece's King is under check, this Piece MUST act upon it. It can either capture the checking piece, block it, or it can't move</remarks>
        public HashSet<Move> EvaluateMoveLegality(HashSet<Move> possibleMoves, Piece piece)
        {
            // Get our King
            King king = GetKing(piece);
            List<Move> illegalMoves = new List<Move>();

            if (!king.IsInCheck)
            {
                // illegalMoves.AddRange(possibleMoves.Where(move => move.RevealsCheck(piece, this)));
                //
                // foreach (var move in illegalMoves)
                // {
                //     possibleMoves.Remove(move);
                // }

                return possibleMoves;
            }

            // Try and get a single attacking piece
            Piece checkingPiece = GetSingleCheckingPiece(piece);

            // if no piece single piece was found, this Piece cannot move to protect the King from both Pieces at once, therefore it cannot move at all
            if (checkingPiece == null)
                return new HashSet<Move>();

            // Get the move's from the single checking Piece
            HashSet<Move> checkingPieceMoves = checkingPiece.GetLegalMoves().Where(m => m.IsDeliveringCheck).ToHashSet();

            return GetBlockingOrCapturingMoves(possibleMoves, checkingPiece, checkingPieceMoves);

        }

        /// <summary>
        /// Gets the required Square coordinates for a Castling move.
        /// </summary>
        /// <param name="king">The king to check against</param>
        /// <param name="isKingSide">If the move is intended to be Kingside or not</param>
        /// <returns>A HashSet of Vector2Int of possible castling squares</returns>
        public HashSet<Vector2Int> GetCastlingSquares(King king, bool isKingSide)
        {
            // Create new HashSet
            HashSet<Vector2Int> requiredSquares = new();
            // Get the Rank (y-coordinate) of the King
            int rank = king.Coordinate.y;

            // These are the standard required-to-be-free Castling squares in a normal Chess layout
            if (isKingSide)
            {
                requiredSquares.Add(new Vector2Int(5, rank));
                requiredSquares.Add(new Vector2Int(6, rank));
            }
            else
            {
                requiredSquares.Add(new Vector2Int(1, rank));
                requiredSquares.Add(new Vector2Int(2, rank));
                requiredSquares.Add(new Vector2Int(3, rank));
            }

            return requiredSquares;
        }

        /// <summary>
        /// Updates the LastMovedPiece
        /// </summary>
        /// <param name="piece">The Piece that has just moved</param>
        public void SetLastMovedPiece(Piece piece)
        {
            LastPieceMoved = piece;
        }

        

        #endregion

        # region INTERNALS

        /// <summary>
        /// Trys to find a a single checking piece
        /// </summary>
        /// <param name="piece">One of our Pieces</param>
        /// <returns>Returns a single Piece that has check on the king, or null if more than one checking piece is found, or no checking pieces are found</returns>
        private Piece GetSingleCheckingPiece(Piece piece)
        {
            // Filter the opponents pieces where they have check on the King
            HashSet<Piece> opponentPieces = GetOpponentPieces(piece).Where(p => p is not King && p.HasCheckOnKing).ToHashSet();

            // if more than one are found, we can't return a single piece, so we return no Piece
            if (opponentPieces.Count != 1) return null;

            // if only one is found, there will be only one in the collection, and therefore can return the first one
            return opponentPieces.First();

        }

        /// <summary>
        /// Gets all of the given Piece's moves that will block or capture a checking piece
        /// </summary>
        /// <param name="possibleMoves">The possible moves this Piece has</param>
        /// <param name="checkingPieces">The Piece that has check on the our King</param>
        /// <param name="checkingMoves">The Piece that has check on our King's moves</param>
        /// <returns>Any legal moves that would block or capture the checking piece</returns>
        private HashSet<Move> GetBlockingOrCapturingMoves(HashSet<Move> possibleMoves, Piece checkingPiece, HashSet<Move> checkingMoves)
        {
            
            HashSet<Move> legalMoves = new HashSet<Move>();

            // iterate through our moves
            foreach (var move in possibleMoves)
            {
                // if the move's target coordinate is the Piece, we can catpure
                if (move.Coordinate == checkingPiece.Coordinate)
                {
                    legalMoves.Add(move);
                    continue;
                }

                // if any of the piece's checking moves equal our move, we can block this move
                if (checkingMoves.Any(m => m.Coordinate == move.Coordinate))
                {
                    legalMoves.Add(move);
                }
            }

            return legalMoves;
        }

        #endregion
    }
}