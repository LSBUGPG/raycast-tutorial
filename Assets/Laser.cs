using System.Collections;
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
            spot.enabled = true;
        }
        else
        {
            spot.enabled = false;
        }
        trail.SetPosition(1, Vector3.forward * distance);
    }
}
