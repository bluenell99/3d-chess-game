using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameModeDescriptionUpdater : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] private Text _headingComponent;
    [SerializeField] private Text _descriptionComponenent;

    [SerializeField] private string _gamemodeHeading;
    [SerializeField] [TextArea] private string _gamemodeDesc;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        _headingComponent.text = _gamemodeHeading;
        _descriptionComponenent.text = _gamemodeDesc;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _headingComponent.text = "";
        _descriptionComponenent.text = "";
    }
}
