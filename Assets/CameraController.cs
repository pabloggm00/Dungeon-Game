using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{


    public Transform player; // El objeto alrededor del cual la cámara rotará
    public float mouseSensivity = 5.0f;

    float rotationX, rotationY;
    float cameraOffset;

    Vector2 mouseMovement;

    private void Start()
    {
        cameraOffset = transform.position.y - player.position.y;
        Cursor.lockState = CursorLockMode.Locked;    
    }

    void Update()
    {
        Vector3 pos = player.position;
        pos.y += cameraOffset;
        transform.position = pos;

        // Obtener la entrada del ratón
        mouseMovement = InputManager.playerControls.Player.Mouse.ReadValue<Vector2>();

        rotationX -= mouseMovement.y * Time.deltaTime * mouseSensivity;
        rotationX = Mathf.Clamp(rotationX, -90, 90);
        rotationY += mouseMovement.x * Time.deltaTime * mouseSensivity;
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        player.rotation = Quaternion.Euler(0, rotationY, 0);

    }

}
