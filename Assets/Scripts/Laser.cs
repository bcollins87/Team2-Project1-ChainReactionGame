using UnityEngine;

public class Laser : MonoBehaviour
{
    public Transform laserStartPoint;  // The start point of the laser (attached to the player)
    public int maxBounces = 5;         // Maximum number of reflections
    public float laserLength = 100f;   // Length of the laser
    public LineRenderer lineRenderer;  // LineRenderer to visualize the laser

    void Update()
    {
        // Check if Mouse 1 is clicked (left mouse button)
        if (Input.GetMouseButtonDown(0))
        {
            // Get the mouse position in the world and calculate the direction
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;

            // Cast a ray from the camera through the mouse position to determine the direction
            if (Physics.Raycast(mouseRay, out mouseHit))
            {
                Vector3 targetDirection = mouseHit.point - laserStartPoint.position;
                targetDirection.y = 0; // Optional: Lock the laser to a 2D plane (e.g., ignore vertical aiming)
                targetDirection.Normalize();

                // Fire the laser in the calculated direction
                FireLaser(laserStartPoint.position, targetDirection);
            }
        }
    }

    void FireLaser(Vector3 position, Vector3 direction)
    {
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, position);

        CastLaser(position, direction, maxBounces);
    }

    void CastLaser(Vector3 position, Vector3 direction, int bouncesLeft)
    {
        if (bouncesLeft == 0) return;

        Ray ray = new Ray(position, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, laserLength))
        {
            // Add the hit point to the LineRenderer
            lineRenderer.positionCount += 1;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

            if (hit.collider.CompareTag("Mirror"))
            {
                // Calculate reflection direction
                Vector3 reflectionDirection = Vector3.Reflect(ray.direction, hit.normal);

                // Recursive call to cast the laser from the new position with the reflected direction
                CastLaser(hit.point, reflectionDirection, bouncesLeft - 1);
            }
            else if (hit.collider.CompareTag("Enemy"))
            {
                // Handle enemy hit
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage();
                }
            }
        }
        else
        {
            // If no collision, extend the laser to its full length
            lineRenderer.positionCount += 1;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, position + direction * laserLength);
        }
    }
}
