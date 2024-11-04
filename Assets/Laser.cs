using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer trail;
    public Renderer spot;

    void Update()
    {
        float distance = Camera.main.farClipPlane;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, distance))
        {
            distance = hit.distance;
            spot.enabled = true;
        }
        else
        {
            spot.enabled = false;
        }
        trail.SetPosition(1, Vector3.forward * distance);
        spot.transform.localPosition = Vector3.forward * distance;
    }
}
