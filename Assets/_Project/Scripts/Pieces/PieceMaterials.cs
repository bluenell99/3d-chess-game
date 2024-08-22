using UnityEngine;

namespace ChessGame
{
    [CreateAssetMenu(menuName = "Chess/Piece Materials")]

    public class PieceMaterials : ScriptableObject
    {
        [field: SerializeField] public Material _whiteMaterial { get; private set; }
        [field: SerializeField] public Material _blackMaterial { get; private set; }
    }
}