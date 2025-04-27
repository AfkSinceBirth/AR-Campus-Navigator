using UnityEngine;
using UnityEngine.UI;

public class CameraFeedController : MonoBehaviour
{
    private RawImage backgroundImage; // No need for public assignment
    private WebCamTexture camTexture;

    void Start()
    {
        // Automatically get RawImage component attached to this GameObject
        backgroundImage = GetComponent<RawImage>();

        if (backgroundImage == null)
        {
            Debug.LogError("RawImage component is missing on this GameObject!");
            return;
        }

        // Get available devices
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            camTexture = new WebCamTexture(devices[0].name); // Use first available camera
            backgroundImage.texture = camTexture; // Set texture to Raw Image
            camTexture.Play(); // Start camera feed
        }
        else
        {
            Debug.LogError("No camera detected!");
        }
    }
}