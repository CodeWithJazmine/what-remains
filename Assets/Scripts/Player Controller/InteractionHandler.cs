using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    private IInteractable currentInteractable;

    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
            currentInteractable.OnPlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && interactable == currentInteractable)
        {
            currentInteractable.OnPlayerExit();
            currentInteractable = null;
        }
    }

    public void TryInteract()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }
}