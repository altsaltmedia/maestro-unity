using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class TextMeshProLinkHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Camera _sourceCamera;

    public Camera sourceCamera {
        get => _sourceCamera;
        set => _sourceCamera = value;
    }

    void Start()
    {
        if(sourceCamera == null) {
            Debug.LogError("Camera must not be null", this);
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("sensing a click");
        TMP_Text pTextMeshPro = GetComponent<TMP_Text>();
        // If you are not in a Canvas using Screen Overlay, put your camera instead of null
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, eventData.position, sourceCamera);
        if (linkIndex != -1) { // was a link clicked?
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }

}