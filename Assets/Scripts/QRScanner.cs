using UnityEngine;
using ZXing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class QRScanner : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    private WebCamTexture camTexture;
    private BarcodeReader barcodeReader;
    private RawImage rawImage;

    void Awake()
    {
        if (camTexture == null) 
        {
            camTexture = new WebCamTexture();
        }
    }

    void Start()
    {
        rawImage = GetComponent<RawImage>(); // Finds the attached RawImage
        StartCoroutine(DelayedStartCamera());
    }

    void OnEnable() // Restart camera when scene is loaded again
    {
        StartCoroutine(DelayedStartCamera());
    }

    IEnumerator DelayedStartCamera()
    {
        yield return new WaitForSeconds(0f); 
        StartCamera();
        rawImage.rectTransform.localEulerAngles = new Vector3(0, 0, -90);
    }

    void StartCamera()
    {
        rawImage.texture = camTexture; 
        rawImage.material.mainTexture = camTexture; 
        camTexture.Play();
        barcodeReader = new BarcodeReader();
    }

    void Update()
    {
        if (camTexture != null && camTexture.isPlaying) //Only scan when camera is active
        {
            TryScanQRCode();
        }
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
        StartCoroutine(LoadNextScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadNextScene(int sceneIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneIndex);
    }

    public bool IsStrictlyAlphanumeric(string input)
    {
        foreach (char c in input)
        {
            if (!char.IsLetterOrDigit(c)) //Checks only letters/numbers
            {
                return false; 
            }
        }
        return true; 
    }

    void OnDisable() 
    {
        if (camTexture != null)
        {
            camTexture.Stop();
        }
    }
}