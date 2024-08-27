using UnityEngine;

namespace ChessGame
{
 public abstract class CaptureStrategy
    {
        /// <summary>
        /// Try and capture a Piece on the Board.
        /// </summary>
        /// <param name="board">The Board in play</param>
        /// <param name="piece">The Piece attempting a capture</param>
        /// <param name="position">The target coordinate</param>
        public abstract void TryCapture(Board board, Piece piece, Vector2Int position);
    }
}