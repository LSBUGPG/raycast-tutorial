using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer trail;
    public Renderer spot;

    void LateUpdate()
    {
        float distance = Camera.main.farClipPlane;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distance))
        {
            distance = hit.distance;
            spot.transform.position = hit.point;
            spot.gameObject.SetActive(true);
        }
        else
        {
            spot.gameObject.SetActive(false);
        }
        trail.SetPosition(1, Vector3.forward * distance);
    }
}
