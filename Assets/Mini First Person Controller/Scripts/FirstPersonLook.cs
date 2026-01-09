using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField]
    Transform character;
    public float sensitivity = 2;
    public float smoothing = 1.5f;

    Vector2 velocity;
    Vector2 frameVelocity;


    void Reset()
    {
        // Get the character from the FirstPersonMovement in parents. 
        character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // Mouse sempre visibile 
        Cursor.visible = false;
    }


    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return; // Non muovere la visuale se il cursore è libero

        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }
}