using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    public abstract class MoveStrategy
    {
        /// <summary>
        /// Gets all possible moves for a given Piece
        /// </summary>
        /// <param name="movement">The directions to move in</param>
        /// <param name="piece">The Piece we want check</param>
        /// <param name="board">Reference to the board</param>
        /// <returns></returns>
        public abstract HashSet<Move> GetPossibleMoves(HashSet<Vector2Int> movement, Piece piece, Board board);
    }
}