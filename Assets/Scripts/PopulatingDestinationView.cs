using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PopulatingDestinationView : MonoBehaviour
{
    public GameObject locationPrefab; // Assign prefab in Inspector
    public Transform contentHolder; // Assign Content GameObject
    public TMP_InputField searchField;

    private List<string> locations = new List<string> { "Office", "Cafeteria", "Meeting Room", "Exit" };

    void Start()
    {
        // Debugging references before running the script
        Debug.Log("locationPrefab assigned: " + (locationPrefab != null));
        Debug.Log("contentHolder assigned: " + (contentHolder != null));
        Debug.Log("searchField assigned: " + (searchField != null));

        if (locationPrefab == null || contentHolder == null || searchField == null)
        {
            Debug.LogError("Critical Error: Missing references! Assign them in the Inspector.");
            return; // Stop execution if references are missing
        }

        PopulateList(locations);
        searchField.onValueChanged.AddListener(FilterList);
    }

    void PopulateList(List<string> locationData)
    {
        foreach (var location in locationData)
        {
            GameObject newItem = Instantiate(locationPrefab, contentHolder);
            
            // Ensure the prefab has a TMP_Text component
            TMP_Text textComponent = newItem.GetComponentInChildren<TMP_Text>();
            if (textComponent != null)
            {
                textComponent.text = location;
            }
            else
            {
                Debug.LogError("Prefab is missing a TMP_Text component!");
            }
        }
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
}