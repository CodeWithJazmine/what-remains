using UnityEngine;

public class ShelterStation : MonoBehaviour, IInteractable
{
    [SerializeField] public StationType stationType;
    [SerializeField] private GameObject interactPopup;

    public bool stationFree = true;

    public void Interact()
    {
        if (GameManager.instance.AssignSurvivorToStation(stationType) == true)
        {
            interactPopup.SetActive(false);
            stationFree = false;
        }
        else
        {
            interactPopup.SetActive(true);
            stationFree = true;
        }

    }

    public void OnPlayerEnter()
    {
        if (stationFree)
        {
            interactPopup.SetActive(true);
        }
    }

    public void OnPlayerExit()
    {
        if (stationFree)
            interactPopup.SetActive(false);
    }
}
