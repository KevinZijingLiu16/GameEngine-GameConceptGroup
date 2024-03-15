using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public RawImage mapImage;
    public Button mapButton;
    public Transform playerTransform;
    public RawImage playerMarker;

    private bool isMapOpen = false;

    void Start()
    {
        mapButton.onClick.AddListener(ToggleMap);
        mapImage.gameObject.SetActive(false);
        playerMarker.gameObject.SetActive(false); // Ensure player marker is initially visible
    }

    void ToggleMap()
    {
        isMapOpen = !isMapOpen;
        mapImage.gameObject.SetActive(isMapOpen);

        // Toggle player marker visibility based on map visibility
        playerMarker.gameObject.SetActive(isMapOpen);

        if (isMapOpen)
        {
            UpdateMapPosition();
        }
    }

    void Update()
    {
        if (isMapOpen)
        {
            UpdateMapPosition();
        }
    }

    void UpdateMapPosition()
    {
        // Get the player's position in world space
        Vector3 playerPosition = playerTransform.position;

        // Convert player's world position to map position
        Vector2 mapPosition = new Vector2(
            (playerPosition.x - mapImage.rectTransform.position.x) / mapImage.rectTransform.rect.width,
            (playerPosition.z - mapImage.rectTransform.position.y) / mapImage.rectTransform.rect.height
        );

        // Convert map position to RawImage coordinates
        Vector2 rawImageSize = mapImage.rectTransform.rect.size;
        Vector2 playerMapPosition = new Vector2(mapPosition.x * rawImageSize.x, mapPosition.y * rawImageSize.y);

        // Set player marker position on the map
        playerMarker.rectTransform.anchoredPosition = playerMapPosition;
    }
}
