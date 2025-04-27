using UnityEngine;
using ZXing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QRScanner : MonoBehaviour
{
    private WebCamTexture camTexture;
    private BarcodeReader barcodeReader;
    private RawImage rawImage; // Automatically finds the attached RawImage

    void Start()
    {
        rawImage = GetComponent<RawImage>(); // 📌 Finds the attached RawImage
        StartCamera();
        rawImage.rectTransform.localEulerAngles = new Vector3(0, 0, -90);
    }

    void StartCamera()
    {
        camTexture = new WebCamTexture();
        rawImage.texture = camTexture; // 📌 Assigns camera feed to UI
        rawImage.material.mainTexture = camTexture; // Ensures correct rendering
        camTexture.Play();
        barcodeReader = new BarcodeReader();
    }

    void Update()
    {
        TryScanQRCode();
    }

    void TryScanQRCode()
    {
        if (camTexture.width > 100 && camTexture.height > 100)
        {
            Color32[] pixels = camTexture.GetPixels32();
            var result = barcodeReader.Decode(pixels, camTexture.width, camTexture.height);

            if (result != null && IsStrictlyAlphanumeric(result.Text))
            {
                Debug.Log($"✅ QR Code Scanned: {result.Text}");
                HandleScannedLocation(result.Text);
            }
        }
    }

    void HandleScannedLocation(string locationId)
    {
        NavigationEndpoints.StartingLocationId = locationId;
        Debug.Log($"📍 Set Player's Start Location to {NavigationEndpoints.StartingLocationId}");
        SceneManager.LoadScene("DestinationSelectionScreen");
    }

    public bool IsStrictlyAlphanumeric(string input)
{
    foreach (char c in input)
    {
        if (!char.IsLetterOrDigit(c)) // ✅ Checks only letters/numbers
        {
            return false; // ❌ Invalid character found!
        }
    }
    return true; // ✅ String is valid!
}

}

// 📌 **Static Class to Store Location Data**
public static class LocationData
{
    public static int StartingLocationId;
    public static int DestinationLocationId;
}