using UnityEngine;

namespace ChessGame
{
    public abstract class CaptureStrategy
    {
        public abstract void TryCapture(Board board, Piece piece, Vector2Int position);
    }
}