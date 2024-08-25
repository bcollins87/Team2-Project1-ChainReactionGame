using UnityEngine;

public class Laser : MonoBehaviour
{
    public Transform laserStartPoint;  // The start point of the laser
    public int maxBounces = 5;         // Maximum number of reflections
    public float laserLength = 100f;   // Length of the laser
    public LineRenderer lineRenderer;  // LineRenderer to visualize the laser
    private bool isFiring = false;
    private Vector3 fireDirection;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isFiring = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isFiring = false;
            lineRenderer.positionCount = 0; // Optionally clear the laser when not firing
        }
        if (isFiring)
        {
            fireDirection = GetMouseDirection();
            if (fireDirection != Vector3.zero)
            {
                FireLaser(laserStartPoint.position, fireDirection);
            }
        }
    }

    Vector3 GetMouseDirection()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out RaycastHit mouseHit, Mathf.Infinity, LayerMask.GetMask("Floor", "Default")))
        {
            Vector3 targetPoint = mouseHit.point;
            targetPoint.y = laserStartPoint.position.y; // Keep the laser at the same y-level as the start point
            return (targetPoint - laserStartPoint.position).normalized;
        }
        return Vector3.zero;
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
            lineRenderer.positionCount += 1;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

            if (hit.collider.CompareTag("Mirror"))
            {
                Vector3 reflectionDirection = Vector3.Reflect(ray.direction, hit.normal);
                CastLaser(hit.point, reflectionDirection, bouncesLeft - 1);
            }
            else if (hit.collider.CompareTag("Glass"))
            {
                CastLaser(hit.point + direction * 0.1f, direction, bouncesLeft);  // Continue through glass
            }
            else if (hit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage();
                }
            }
        }
        else
        {
            lineRenderer.positionCount += 1;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, position + direction * laserLength);
        }
    }
}
