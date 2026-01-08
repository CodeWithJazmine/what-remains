using UnityEngine;
using UnityEngine.UIElements;

public class InteractPopup : MonoBehaviour
{
    [SerializeField] private Texture2D interactIconTexture;
    [SerializeField] private UIDocument uiDocument;
    private VisualElement interactIcon;

    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        interactIcon = root.Q<VisualElement>("InteractIcon");
        interactIcon.style.backgroundImage = interactIconTexture;
    }
}
