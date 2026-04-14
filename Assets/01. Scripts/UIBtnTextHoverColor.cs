using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIBtnTextHoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI targetText;

    private Color hoverColor;
    private Color originColor;

    private void Awake()
    {
        targetText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        originColor = targetText.color;

        Color color;
        if (ColorUtility.TryParseHtmlString("#9EBC43", out color))
        {
            hoverColor = color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetText.color = originColor;
    }
}
