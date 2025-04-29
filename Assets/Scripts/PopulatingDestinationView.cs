using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Android;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PopulatingDestinationView : MonoBehaviour
{

    public Animator transition;
    public float transitionTime = 1f;
    public GameObject locationPrefab; // Button prefab for locations
    public Transform contentHolder; // Parent object for scroll view items
    public TMP_InputField searchField; // Search bar input field

    private DatabaseManager databaseManager; // Handles database interactions
    private List<(string, string)> locations; // Stores retrieved locations

    void Start()
    {
        databaseManager = new DatabaseManager(); // 📌 Create Database Manager instance
        PopulateScrollView();
        searchField.onValueChanged.AddListener(FilterList);
    }

    // 📌 **Fetch Location Data and Create Buttons**
    void PopulateScrollView()
    {
        locations = databaseManager.GetLocations(); // 📌 Fetch both Location_ID & Location_Name

        foreach ((string, string) location in locations)
        {
            GameObject newItem = Instantiate(locationPrefab, contentHolder);
            Button button = newItem.GetComponent<Button>();
            TMP_Text nameText = newItem.transform.Find("name")?.GetComponent<TMP_Text>();
            TMP_Text distanceText = newItem.transform.Find("distance")?.GetComponent<TMP_Text>();

            if (nameText != null && distanceText != null)
            {
                (string id, string name) = location;

                nameText.text = name; // 📌 Assign location name
                distanceText.text = "0m"; // Placeholder distance

                // 📌 **Attach Location_ID directly to the button**
                LocationButton buttonScript = newItem.AddComponent<LocationButton>();
                buttonScript.Initialize(id);

                // 📌 **On Click, store selected Location_ID**
                button.onClick.AddListener(() => OnLocationSelected(id, name));

                Debug.Log($"✅ Added location: {name} (ID: {id})");
            }
            else
            {
                Debug.LogError("❌ TMP_Text components not found in prefab! Check object names.");
            }
        }
    }

    // 📌 **Store Selected Location_ID in `NavigationEndpoints`**
    void OnLocationSelected(string destinationId, string destinationName)
    {
        NavigationEndpoints.DestinationLocationId = destinationId;
        NavigationEndpoints.DestinationLocationName = destinationName;
        Debug.Log($"✅ Destination Selected: {NavigationEndpoints.DestinationLocationId}");
        StartCoroutine(LoadNextScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadNextScene(int sceneIndex)
    {

        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneIndex);
    }

    void FilterList(string query)
    {
        foreach (Transform item in contentHolder)
        {
            TMP_Text textComponent = item.GetComponentInChildren<TMP_Text>();
            if (textComponent != null)
            {
                bool match = textComponent.text.ToLower().Contains(query.ToLower());
                item.gameObject.SetActive(match);
            }
        }
    }

    // 📌 **Stores Location_ID Inside Button**
    public class LocationButton : MonoBehaviour
    {
        public string LocationID { get; private set; }

        public void Initialize(string locationId)
        {
            LocationID = locationId; // ✅ Stores Location_ID for later retrieval
        }
    }
    public void goBack()
    {
        StartCoroutine(LoadNextScene(SceneManager.GetActiveScene().buildIndex - 1));
    }
}