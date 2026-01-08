using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    private CharacterController characterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        //if (!characterController) Debug.LogWarning("ThirdPersonMovement: Can't get character controller");
    }

    public void Move(Vector2 input, float speed)
    {
        Vector3 move = (transform.right * input.x) + transform.forward * input.y;
        move = Vector3.ClampMagnitude(move, 1f);
        characterController.Move(Time.deltaTime * speed * move);
    }

}
