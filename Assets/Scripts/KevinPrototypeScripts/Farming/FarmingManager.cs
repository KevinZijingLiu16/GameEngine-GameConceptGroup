using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmingManager : MonoBehaviour
{
    [Header("Farming Objects")]
    [SerializeField] private List<GameObject> treeObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> flowerObjects = new List<GameObject>();

    [Header("Farming Settings")]
    [SerializeField] private SelectedFarmType currentFarmType;
    [SerializeField] private Material ghostMaterialValid;
    [SerializeField] private Material ghostMaterialInvalid;
    [SerializeField] private float maxPlantingAngle = 45f;

    [Header("Internal State")]
    [SerializeField] private bool isFarming = false;
    [SerializeField] private int currentPlantIndex;
    private GameObject ghostPlantGameObject;
    private bool isGhostInvalidPlantPosition = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isFarming = !isFarming;
            if (isFarming)
            {
                ghostPlant();
            }
            else if (ghostPlantGameObject)
            {
                Destroy(ghostPlantGameObject);
                ghostPlantGameObject = null;
            }
        }

        if (isFarming && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left mouse button clicked in farming mode!");
            plantCrop();
        }

        // Switch between different types of crops (trees, flowers)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentFarmType = SelectedFarmType.trees;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentFarmType = SelectedFarmType.flowers;
        }
    }

    private void ghostPlant()
    {
        GameObject currentCrop = getCurrentCrop();
        createGhostCropPrefab(currentCrop);

        moveGhostCropPrefabToRaycast();
        checkPlantValidity();
    }

    private void createGhostCropPrefab(GameObject currentCrop)
    {
        if (ghostPlantGameObject == null)
        {
            ghostPlantGameObject = Instantiate(currentCrop);

            Transform ModelParent = ghostPlantGameObject.transform.GetChild(0);

            ghostifyModel(ModelParent, ghostMaterialValid);
            ghostifyModel(ghostPlantGameObject.transform);
        }
    }

    private void moveGhostCropPrefabToRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            ghostPlantGameObject.transform.position = hit.point;
            Debug.Log("Ghost plant position set to: " + ghostPlantGameObject.transform.position);
        }
    }

    private void checkPlantValidity()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (Vector3.Angle(hit.normal, Vector3.up) < maxPlantingAngle)
            {
                ghostifyModel(ghostPlantGameObject.transform, ghostMaterialValid);
                isGhostInvalidPlantPosition = true;
                Debug.Log("Ghost plant is in a valid position.");
            }
            else
            {
                ghostifyModel(ghostPlantGameObject.transform, ghostMaterialInvalid);
                isGhostInvalidPlantPosition = false;
                Debug.Log("Ghost plant is in an invalid position (angle).");
            }
        }
    }

    private GameObject getCurrentCrop()
    {
        switch (currentFarmType)
        {
            case SelectedFarmType.trees:
                return treeObjects[currentPlantIndex];
            case SelectedFarmType.flowers:
                return flowerObjects[currentPlantIndex];
                // Add more cases if you have other types of crops
        }
        return null;
    }

    private void plantCrop()
    {
        Debug.Log("Attempting to plant crop...");
        if (ghostPlantGameObject != null && isGhostInvalidPlantPosition)
        {
            Debug.Log("Planting crop...");

            // Raycast to find the ground height at the planting position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                float groundHeight = hit.point.y;

                // Set the Y position of the prefab to the ground height
                Vector3 plantPosition = new Vector3(ghostPlantGameObject.transform.position.x, groundHeight, ghostPlantGameObject.transform.position.z);

                // Instantiate the crop at the adjusted position
                Instantiate(getCurrentCrop(), plantPosition, ghostPlantGameObject.transform.rotation);
            }
            else
            {
                Debug.Log("Failed to find ground height. Planting at the original position.");
                Instantiate(getCurrentCrop(), ghostPlantGameObject.transform.position, ghostPlantGameObject.transform.rotation);
            }

            Destroy(ghostPlantGameObject);
            ghostPlantGameObject = null;

            isFarming = false;

            // Additional farming-related logic can be added here
        }
        else
        {
            Debug.Log("Failed to plant crop. Conditions not met.");
        }
    }


    private void ghostifyModel(Transform model, Material ghostMaterial = null)
    {
        if (ghostMaterial != null)
        {
            foreach (MeshRenderer meshRenderer in model.GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material = ghostMaterial;
            }
        }
        else
        {
            foreach (Collider modelColliders in model.GetComponentsInChildren<Collider>())
            {
                modelColliders.enabled = false;
            }
        }
    }
}

[System.Serializable]
public enum SelectedFarmType
{
    trees,
    flowers,
    // Add more types if needed
}
