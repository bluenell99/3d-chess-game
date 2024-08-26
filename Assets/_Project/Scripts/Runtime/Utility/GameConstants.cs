using System.Collections.Generic;

public static class GameConstants
{
    public const int BOARD_SIZE = 8;

    public static Dictionary<PieceType, int> MaterialCostsDictionary = new()
    {
        { PieceType.Pawn, 1 },
        { PieceType.Knight, 3 },
        { PieceType.Bishop, 3 },
        { PieceType.Rook, 5 },
        { PieceType.Queen, 9 },
        { PieceType.King, 0 },
    };

}