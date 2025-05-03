using UnityEngine;
using SQLite4Unity3d;
using System.IO;
using System.Collections.Generic;

public class DatabaseManager
{
    private SQLiteConnection _connection;
    private string dbPath;

    public DatabaseManager()
    {
        dbPath = Path.Combine(Application.persistentDataPath, "locations.db");

        if (!File.Exists(dbPath))
        {
            Debug.LogWarning($" Database not found in persistent storage. Copying from StreamingAssets...");
            CopyDatabaseFromStreamingAssets();
        }

    
        if (!File.Exists(dbPath))
        {
            Debug.LogError($"Database file not found at {dbPath}!");
            return;
        }

        _connection = new SQLiteConnection(dbPath);
        Debug.Log($"Connected to database at: {dbPath}");
    }

   
    private void CopyDatabaseFromStreamingAssets()
    {
        string sourcePath = Path.Combine(Application.streamingAssetsPath, "locations.db");

        try
        {
#if UNITY_ANDROID
            using (UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(sourcePath))
            {
                www.SendWebRequest();
                while (!www.isDone) { } // Wait for download to complete
                File.WriteAllBytes(dbPath, www.downloadHandler.data);
            }
#else
       
        File.Copy(sourcePath, dbPath, true); 
#endif
            Debug.Log($"Database replaced successfully at: {dbPath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to copy database: {ex.Message}");
        }
    }
    public List<(string LocationID, string LocationName)> GetLocations()
    {
        List<(string, string)> locations = new List<(string, string)>();

        try
        {
            var query = _connection.Query<LocationModel>("SELECT Location_ID, Location_Name FROM Locations;");

            foreach (var location in query)
            {
                locations.Add((location.Location_ID, location.Location_Name)); 
            }

            Debug.Log($" Retrieved {locations.Count} locations.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($" Error fetching locations: {ex.Message}");
        }

        return locations;
    }
}


public class LocationModel
{
    public string Location_ID { get; set; } 
    public string Location_Name { get; set; } 
}