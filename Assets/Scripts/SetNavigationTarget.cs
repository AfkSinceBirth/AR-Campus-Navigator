using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SetNavigationTarget : MonoBehaviour
{
    [SerializeField]
    private GameObject navTargetObject;

    [SerializeField]
    private float groundSearchDistance = 2.0f;

    [SerializeField]
    private float offsetHeight = 1.0f;

    private NavMeshPath path; // Current calculated path
    private LineRenderer line; // LineRenderer to display path

    private bool pathVisible = false; // Toggle for showing/hiding path

    private void Start()
    {
        path = new NavMeshPath();
        line = GetComponent<LineRenderer>();

        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit playerHit, 2.0f, NavMesh.AllAreas))
        {
            Debug.LogWarning("Player is NOT on the NavMesh!");
        }

        if (!NavMesh.SamplePosition(navTargetObject.transform.position, out NavMeshHit targetHit, 2.0f, NavMesh.AllAreas))
        {
            Debug.LogWarning("Target object is NOT on the NavMesh!");
        }

        // Set up the LineRenderer
        line.enabled = false;
    }

    private Vector3 AdjustToNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, groundSearchDistance, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return position;
    }

    private void Update()
    {

        if (pathVisible)
        {
            RenderPath();
        }
        else
        {
            line.enabled = false; // Hide path when toggle is off
        }
    }

    public void TogglePathVisibility()
    {
        pathVisible = !pathVisible;
    }

    private void RenderPath()
    {
        // Calculate ground-projected start (player) and target positions
        Vector3 startPos = AdjustToNavMesh(transform.position);
        Vector3 targetPos = AdjustToNavMesh(navTargetObject.transform.position);

        // Generate the path
        NavMesh.CalculatePath(startPos, targetPos, NavMesh.AllAreas, path);

        if (path.status == NavMeshPathStatus.PathComplete && path.corners.Length > 0)
        {
            Vector3[] floatingCorners = new Vector3[path.corners.Length];
            for (int i = 0; i < path.corners.Length; i++)
            {
                Vector3 corner = path.corners[i];
                if (i == 0)
                {
                    floatingCorners[i] = new Vector3(corner.x, transform.position.y, corner.z);
                }
                else if (i == path.corners.Length - 1)
                {
                    floatingCorners[i] = new Vector3(corner.x, navTargetObject.transform.position.y, corner.z);
                }
                else
                {
                    floatingCorners[i] = new Vector3(corner.x, corner.y + offsetHeight, corner.z);
                }
            }

            line.positionCount = floatingCorners.Length;
            line.SetPositions(floatingCorners);
            line.enabled = true;
        }
        else
        {
            line.enabled = false;
            Debug.LogWarning("NavMesh path calculation failed: " + path.status);
        }
    }
}