using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    //private VisualElement inventoryMenu;
    private VisualElement survivorPhoto;
    private Label survivorName;
    private Label phaseIntro;
    [SerializeField] private float popupTiming = 4f;
    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        //inventoryMenu = root.Q<VisualElement>("InventoryPanel");
        survivorPhoto = root.Q<VisualElement>("SurvivorPhoto");
        survivorName = root.Q<Label>("SurvivorName");
        phaseIntro = root.Q<Label>("PhaseIntro");
    }

    public void UpdateSurvivorPanel(Survivor survivor)
    {
        survivorPhoto.style.backgroundImage = survivor.survivorData.survivorIcon;
        survivorName.text = survivor.survivorData.survivorName;
    }

    public IEnumerator ShowPhaseIntro(int phaseCount)
    {
        TogglePhaseIntroText();

        if (phaseCount % 2 == 0) // Day
        {
            phaseIntro.text = $"Day {phaseCount + 1}";
        }
        else if (phaseCount % 2 == 1) // Night
        {
            phaseIntro.text = $"Night {phaseCount}";
        }

        yield return new WaitForSeconds(popupTiming);

        TogglePhaseIntroText();
    }

    void TogglePhaseIntroText()
    {
        if (phaseIntro.ClassListContains("hide"))
        {
            phaseIntro.RemoveFromClassList("hide");
        }
        else
        {
            phaseIntro.AddToClassList("hide");
        }
    }

    // public void ToggleInventoryMenu()
    // {
    //     if (inventoryMenu.ClassListContains("hide"))
    //     {
    //         inventoryMenu.RemoveFromClassList("hide");
    //     }
    //     else
    //     {
    //         inventoryMenu.AddToClassList("hide");
    //     }
    // }
}
