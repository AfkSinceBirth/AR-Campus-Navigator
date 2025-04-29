using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class SetNavigationTarget : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        SceneManager.sceneUnloaded += (scene) => {
            LoaderUtility.Deinitialize();
            LoaderUtility.Initialize();
        };
    }

    public Animator transition;
    public float transitionTime = 1f;

    [SerializeField]
    private float groundSearchDistance = 2.0f;

    [SerializeField]
    private float offsetHeight = 1.0f;

    private NavMeshPath path; // Current calculated path
    private LineRenderer line; // LineRenderer to display path

    private bool pathVisible = false; // Toggle for showing/hiding path

    private GameObject navStartObject;
    private GameObject navTargetObject;

    private void Start()
    {
        path = new NavMeshPath();
        line = GetComponent<LineRenderer>();

        GameObject info = GameObject.Find("DestinationInfo");
        TMP_Text tmpText = info.GetComponent<TMP_Text>();
        tmpText.text = $"To {NavigationEndpoints.DestinationLocationName}";

        Debug.Log($"start : {NavigationEndpoints.StartingLocationId}  end : {NavigationEndpoints.DestinationLocationId}");
        navStartObject = GameObject.Find(NavigationEndpoints.StartingLocationId);
        navTargetObject = GameObject.Find(NavigationEndpoints.DestinationLocationId);
        UpdatePosition(gameObject, navStartObject);

        GetPathDistance();

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
            GetPathDistance();
            RenderPath();
        }
        else
        {
            line.enabled = false; // Hide path when toggle is off
        }
    }
    void UpdatePosition(GameObject current, GameObject target)
    {
        Vector3 newPosition = new Vector3(target.transform.position.x, current.transform.position.y, target.transform.position.z);
        current.transform.position = newPosition;
    }

    public void GetPathDistance()
    {   
        Vector3 startPos = AdjustToNavMesh(transform.position);
        Vector3 targetPos = AdjustToNavMesh(navTargetObject.transform.position);

        if (NavMesh.CalculatePath(startPos, targetPos, NavMesh.AllAreas, path))
        {
            float totalDistance = 0f;
            for (int i = 1; i < path.corners.Length; i++) // Loop through path points
            {
                totalDistance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
            if(totalDistance < 0.5 ) {
                StartCoroutine(LoadNextScene(SceneManager.GetActiveScene().buildIndex + 1));
            }
            GameObject textObject = GameObject.Find("DistanceLeftValue");
            TMP_Text tmpText = textObject.GetComponent<TMP_Text>();
            tmpText.text = $"{totalDistance:F1}m";
        }
        else
        {
            Debug.LogWarning("NavMesh path calculation failed: " + path.status);
        }

    }

    IEnumerator LoadNextScene(int sceneIndex)
    {

        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneIndex);
    }

    public void RenderPath()
    {   
        pathVisible = true;
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

    public void goBack(){
        StartCoroutine(LoadNextScene(SceneManager.GetActiveScene().buildIndex - 1));
    }
}