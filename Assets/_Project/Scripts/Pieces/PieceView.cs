using UnityEngine;
using UnityEngine.Serialization;

public class PieceView : MonoBehaviour, ISelectable
{

    [FormerlySerializedAs("checkDetector")] [SerializeField] private GameObject checkIndicator;
    private MeshRenderer _meshRenderer;

    public PieceController Controller { get; private set; }
    
    public void Initialise(PieceController controller, Material material, PieceColor color, PieceType type)
    {
        Controller = controller;
        
        SetMaterial(material);
        SetRotation(color, type);
    }

    private void SetRotation(PieceColor color, PieceType type)
    {
        if (type != PieceType.Knight)
            return;

        float rotationAmount = color == PieceColor.White ? 180f : 0;
        Quaternion rotation = Quaternion.Euler(0, rotationAmount, 0);
        transform.rotation = rotation;
    }

    public void SetPosition(Vector3 position)
    {
        transform.localPosition = position;
    }

    private  void SetMaterial(Material material)
    {
        if (_meshRenderer == null)
            _meshRenderer = GetComponentInChildren<MeshRenderer>();

        _meshRenderer.material = material;
    }

    public void OnSelect()
    {
        if ((GameController.Instance.TurnsEnabled && Controller.Piece.PieceColor != GameController.Instance.CurrentTurn) || GameController.Instance.PawnPromotionInProgress)
            return;
        
        
        GameController.Instance.SetSelectedPiece(this);

       
    }

    public void EnableCheckIndicator(bool enable)
    {
        if (checkIndicator != null)
        {
            checkIndicator.SetActive(enable);
        }
    }
    
}

public interface ISelectable
{
    public void OnSelect();
}