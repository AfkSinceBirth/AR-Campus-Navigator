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

        // 🔍 Ensure database exists before connecting
        if (!File.Exists(dbPath))
        {
            Debug.LogError($"❌ Database file not found at {dbPath}!");
            return;
        }

        _connection = new SQLiteConnection(dbPath);
        Debug.Log($"✅ Connected to database at: {dbPath}");
    }

    // 📌 **Fetch Only Location Names (Compatible with PopulatingDestinationView)**
    public List<(string LocationID, string LocationName)> GetLocations()
    {
        List<(string, string)> locations = new List<(string, string)>();

        try
        {
            var query = _connection.Query<LocationModel>("SELECT Location_ID, Location_Name FROM Locations;");

            foreach (var location in query)
            {
                locations.Add((location.Location_ID, location.Location_Name)); // ✅ Store both values in tuple
            }

            Debug.Log($"✅ Retrieved {locations.Count} locations.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Error fetching locations: {ex.Message}");
        }

        return locations; // ✅ Returns both ID & Name together
    }
}

// 📌 **Minimal Class for SQLite Mapping**
public class LocationModel
{
    public string Location_ID { get; set; } // ✅ Stores Location ID
    public string Location_Name { get; set; } // ✅ Stores Location Name
}