using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float sensitivity = 30;
    public float debugSpeed = 0.1f;
    float pitch = 0;
    float yaw = 0;

    void Update()
    {
        pitch -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        yaw += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }
}
