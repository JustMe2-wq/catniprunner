using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float yOffset = 3f;

    void LateUpdate()
    {
        Vector3 camPos = transform.position;
        camPos.x = player.position.x;
        camPos.y = player.position.y + yOffset;
        transform.position = camPos;
    }
}