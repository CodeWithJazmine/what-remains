using UnityEngine;

public interface IInteractable
{
    // Called when the player interacts with this object.
    void Interact();

    // (Optional) Called when the player enters the interaction trigger. Useful for showing prompts like "Press E to interact".
    void OnPlayerEnter();

    // (Optional) Called when the player exits the interaction trigger. Useful for hiding prompts or resetting states.
    void OnPlayerExit();
}