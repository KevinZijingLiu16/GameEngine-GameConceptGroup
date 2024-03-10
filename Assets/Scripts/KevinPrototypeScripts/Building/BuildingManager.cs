using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [Header("Build Objects")]
    [SerializeField] private List<GameObject> floorObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> wallObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> doorObjects = new List<GameObject>(); // Add doors

    [Header("Build Settings")]
    [SerializeField] private SelectedBuildType currentBuildType;
    [SerializeField] private LayerMask connectorLayer;

    [Header("Ghost Settings")]
    [SerializeField] private Material ghostMaterialValid;
    [SerializeField] private Material ghostMaterialInvalid;
    [SerializeField] private float connectorOverlapRadius = 2;
    [SerializeField] private float maxGroundAngle = 45f;

    [Header("Internal State")]
    [SerializeField] private bool isBuilding = false;
    [SerializeField] private int currentBuildIndex;
    private GameObject ghostBuildGameObject;
    private bool isGhostInvalidPosition = false;
    private Transform ModelParent = null;


    private void Update()
    {
        // Switch between building types using keys 1, 2, and 3
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentBuildType = SelectedBuildType.floor;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentBuildType = SelectedBuildType.wall;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentBuildType = SelectedBuildType.door;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            isBuilding = !isBuilding;
            if (isBuilding)
            {
                ghostBuild();
            }
            else if (ghostBuildGameObject)
            {
                Destroy(ghostBuildGameObject);
                ghostBuildGameObject = null;
            }
        }

       
        if (isBuilding && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left mouse button clicked!");
            placeBuild();
        }
    }





    private void ghostBuild()
    {
        GameObject currentBuild = getCurrentBuild();
        createGhostPrefab(currentBuild);

        moveGhostPrefabToRaycast();
        checkBuildValidity();
    }

    private void createGhostPrefab(GameObject currentBuild)
    {
        if (ghostBuildGameObject == null)
        {
            ghostBuildGameObject = Instantiate(currentBuild);

            ModelParent = ghostBuildGameObject.transform.GetChild(0);

            ghostifyModel(ModelParent, ghostMaterialValid);
            ghostifyModel(ghostBuildGameObject.transform);
        }
    }

    private void moveGhostPrefabToRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            ghostBuildGameObject.transform.position = hit.point;
            Debug.Log("Ghost position set to: " + ghostBuildGameObject.transform.position);
            //ghostBuildGameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
    }


    private void checkBuildValidity()
    {
        Collider[] colliders = Physics.OverlapSphere(ghostBuildGameObject.transform.position, connectorOverlapRadius, connectorLayer);
        if (colliders.Length > 0)
        {
            ghostConnectBuild(colliders);
        }
        else
        {
            ghostSeperateBuild();
        }
    }


    private void ghostConnectBuild(Collider[] colliders)
    {
        Connector bestConnector = null;

        foreach (Collider collider in colliders)
        {
            Connector connector = collider.GetComponent<Connector>();

            if (connector.canConnectTo)
            {
                bestConnector = connector;
                break;
            }
        }

        if (bestConnector == null || (currentBuildType == SelectedBuildType.floor && bestConnector.isConnectedToFloor) || (currentBuildType == SelectedBuildType.wall && bestConnector.isConnectedToWall))
        {
            ghostifyModel(ModelParent, ghostMaterialInvalid);
            isGhostInvalidPosition = false;
            Debug.Log("Ghost is in an invalid position.");
            return;
        }

        snapGhostPrefabToConnector(bestConnector);
        isGhostInvalidPosition = true; // Set to true for a valid position
    }



    private void snapGhostPrefabToConnector(Connector connector)
    {
        Transform ghostConnector = findSnapConnector(connector.transform, ghostBuildGameObject.transform.GetChild(1));

        Vector3 newPosition = connector.transform.position - (ghostConnector.position - ghostBuildGameObject.transform.position);

        if (currentBuildType == SelectedBuildType.wall)
        {
            // Offset the wall's position based on the connector's normal
            float offset = ghostBuildGameObject.transform.localScale.y / 2; // Adjust this value as needed
            newPosition += connector.transform.up * offset;
        }

        ghostBuildGameObject.transform.position = newPosition;

        if (currentBuildType == SelectedBuildType.wall)
        {
            Quaternion newRotation = ghostBuildGameObject.transform.rotation;
            newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, connector.transform.rotation.eulerAngles.y, newRotation.eulerAngles.z);
            ghostBuildGameObject.transform.rotation = newRotation;
        }

        ghostifyModel(ModelParent, ghostMaterialValid);
        isGhostInvalidPosition = true;
    }


    private void ghostSeperateBuild()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (currentBuildType == SelectedBuildType.wall)
            {
                ghostifyModel(ModelParent, ghostMaterialInvalid);
                isGhostInvalidPosition = false;
                Debug.Log("Ghost is in an invalid position (wall).");
                return;
            }

            if (hit.collider.transform.root.CompareTag("Buildables"))
            {
                ghostifyModel(ModelParent, ghostMaterialInvalid);
                isGhostInvalidPosition = false;
                Debug.Log("Ghost is in an invalid position (Buildables tag).");
                return;
            }

            if (Vector3.Angle(hit.normal, Vector3.up) < maxGroundAngle)
            {
                ghostifyModel(ModelParent, ghostMaterialValid);
                isGhostInvalidPosition = true; // Set to true for a valid position
                Debug.Log("Ghost is in a valid position.");
            }
            else
            {
                ghostifyModel(ModelParent, ghostMaterialInvalid);
                isGhostInvalidPosition = false;
                Debug.Log("Ghost is in an invalid position (angle).");
            }
        }
    }

    private Transform findSnapConnector(Transform snapConnector, Transform ghostConnectorParent)
    {
        ConnectorPosition OppositeConnetorTag = getOppositePosition(snapConnector.GetComponent<Connector>());

        foreach (Connector connector in ghostConnectorParent.GetComponentsInChildren<Connector>())
        {
            if(connector.connectorPosition == OppositeConnetorTag)
            {
                return connector.transform;
            }
        }
       
        return null;
    }

    private ConnectorPosition getOppositePosition(Connector connector)
    {
       ConnectorPosition position = connector.connectorPosition;

       if(currentBuildType == SelectedBuildType.wall && connector.connectorParentType == SelectedBuildType.floor)
        {
          return ConnectorPosition.bottom;
       }
        if (currentBuildType == SelectedBuildType.floor && connector.connectorParentType == SelectedBuildType.wall && connector.connectorPosition == ConnectorPosition.top )
        {
            if (connector.transform.root.rotation.y == 0)
            {
                return getConnectorClosestToPlayer(true);
            }
            else
            {
                return getConnectorClosestToPlayer(false);
            }
        }

        switch (position)
        {
            case ConnectorPosition.left:
                return ConnectorPosition.right;
            case ConnectorPosition.right:
                return ConnectorPosition.left;
            case ConnectorPosition.top:
                return ConnectorPosition.bottom;
            case ConnectorPosition.bottom:
                return ConnectorPosition.top;
            default:
                return ConnectorPosition.bottom;
        }
    }

    private ConnectorPosition getConnectorClosestToPlayer(bool topBottom)
    {
        Transform cameraTransform = Camera.main.transform;

        if(topBottom)
            return cameraTransform.position.z >= ghostBuildGameObject.transform.position.z ? ConnectorPosition.bottom : ConnectorPosition.top;
        else
            return cameraTransform.position.x >= ghostBuildGameObject.transform.position.x ? ConnectorPosition.left : ConnectorPosition.right;
    }

    private void ghostifyModel(Transform modelParent, Material ghostMaterial = null)
    {
        if( ghostMaterial != null)
        {
            foreach (MeshRenderer meshRenderer in modelParent.GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material = ghostMaterial;
            }
        }
        else
        {
            foreach (Collider modelColliders in modelParent.GetComponentsInChildren<Collider>())
            {
               modelColliders.enabled = false;
            }
        }
        
    }

    private GameObject getCurrentBuild()
    {
        switch (currentBuildType)
        {
            case SelectedBuildType.floor:
                return floorObjects[currentBuildIndex];
            case SelectedBuildType.wall:
                return wallObjects[currentBuildIndex];
            case SelectedBuildType.door:
                return doorObjects[currentBuildIndex];
        }
        return null;
    }

    private void placeBuild()
    {
        Debug.Log("Attempting to place build...");
        if (ghostBuildGameObject != null && isGhostInvalidPosition)
        {
            Debug.Log("Placing build...");
            GameObject newBuild = Instantiate(getCurrentBuild(), ghostBuildGameObject.transform.position, ghostBuildGameObject.transform.rotation);

            Destroy(ghostBuildGameObject);
            ghostBuildGameObject = null;

            isBuilding = false;

            foreach (Connector connector in newBuild.GetComponentsInChildren<Connector>())
            {
                connector.updateConnectors(true);
            }
        }
        else
        {
            Debug.Log("Failed to place build. Conditions not met.");
        }
    }



}

[System.Serializable]
public enum SelectedBuildType
{
    floor,
    wall,
    door

   
}
