using UnityEngine;

public class HideBetweenCarAndCamera : MonoBehaviour
{
    public Transform car; // Reference to the car transform
    public LayerMask ignoreLayer; // Layer mask to ignore for camera collision

    private Camera mainCamera; // Reference to the main camera
    private RaycastHit[] previousHits; // Previous objects hit by raycast

    void Start()
    {
        mainCamera = Camera.main;
        previousHits = new RaycastHit[0];
    }

    void Update()
    {
        if (car == null || mainCamera == null)
            return;

        // Raycast from car to camera position
        Vector3 direction = mainCamera.transform.position - car.position;
        RaycastHit[] hits = Physics.RaycastAll(car.position, direction.normalized, direction.magnitude, ignoreLayer);

        // Re-enable objects that are no longer hit
        foreach (var hit in previousHits)
        {
            if (!IsInArray(hit, hits))
            {
                Renderer renderer = hit.collider.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = true;
                }
            }
        }

        // Hide new objects and update previous hits
        foreach (var hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }

        previousHits = hits;
    }

    bool IsInArray(RaycastHit hit, RaycastHit[] array)
    {
        foreach (var h in array)
        {
            if (h.collider == hit.collider)
                return true;
        }
        return false;
    }
}
