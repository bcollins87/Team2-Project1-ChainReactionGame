using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public MirrorPlacement mirrorPlacement; // Reference to the MirrorPlacement script

    void Update()
    {
        // Check for mirror replacement action (using mouse right-click)
        if (mirrorPlacement != null && Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mirrorPlacement.placementLayer))
            {
                if (hit.collider.CompareTag("Floor"))
                {
                    // Replace the first picked-up mirror from the list
                    mirrorPlacement.ReplaceMirror(hit.point);
                }
            }
        }
    }
}
