using System.Collections.Generic;
using UnityEngine;

namespace ChessGame
{
    public static class Utilities
    {
        /// <summary>
        /// Converts algebraic notation piece coordinates, to usable Vector2Int coordinates the game runs on
        /// </summary>
        /// <param name="algebraic"></param>
        /// <returns></returns>
        public static Vector2Int FromAlgebraicNotation(string algebraic)
        {
            int x = algebraic[0] - 'a';
            int y = algebraic[1] - '1';
            return new Vector2Int(x, y);
        }

        public static Vector2Int[] AlgebraicMoveToVector2IntArray(string algebraic)
        {
            Vector2Int from = FromAlgebraicNotation(algebraic.Substring(0, 2));
            Vector2Int to = FromAlgebraicNotation(algebraic.Substring(2, 2));

            Vector2Int[] move = new[]
            {
                from,
                to
            };

            return move;

        }
    }
}