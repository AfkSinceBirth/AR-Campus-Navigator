using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PopulatingDestinationView : MonoBehaviour
{

    public Animator transition;
    public float transitionTime = 1f;
    public GameObject locationPrefab; 
    public Transform contentHolder; 
    public TMP_InputField searchField;

    private DatabaseManager databaseManager; 
    private List<(string, string)> locations; 

    void Start()
    {
        databaseManager = new DatabaseManager(); 
        PopulateScrollView();
        searchField.onValueChanged.AddListener(FilterList);
    }

    
    void PopulateScrollView()
    {
        locations = databaseManager.GetLocations(); 

        foreach ((string, string) location in locations)
        {
            GameObject newItem = Instantiate(locationPrefab, contentHolder);
            Button button = newItem.GetComponent<Button>();
            TMP_Text nameText = newItem.transform.Find("name")?.GetComponent<TMP_Text>();
            TMP_Text distanceText = newItem.transform.Find("distance")?.GetComponent<TMP_Text>();

            if (nameText != null && distanceText != null)
            {
                (string id, string name) = location;

                nameText.text = name; 
                distanceText.text = "0m"; 

                
                LocationButton buttonScript = newItem.AddComponent<LocationButton>();
                buttonScript.Initialize(id);

                
                button.onClick.AddListener(() => OnLocationSelected(id, name));

                Debug.Log($"Added location: {name} (ID: {id})");
            }
            else
            {
                Debug.LogError(" TMP_Text components not found in prefab! Check object names.");
            }
        }
    }

   
    void OnLocationSelected(string destinationId, string destinationName)
    {
        NavigationEndpoints.DestinationLocationId = destinationId;
        NavigationEndpoints.DestinationLocationName = destinationName;
        Debug.Log($" Destination Selected: {NavigationEndpoints.DestinationLocationId}");
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

    public class LocationButton : MonoBehaviour
    {
        public string LocationID { get; private set; }

        public void Initialize(string locationId)
        {
            LocationID = locationId;
        }
    }
    public void goBack()
    {
        StartCoroutine(LoadNextScene(SceneManager.GetActiveScene().buildIndex - 1));
    }
}