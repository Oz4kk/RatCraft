using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCubePointer : MonoBehaviour
{
    //SerializeFields
    [SerializeField] private GameObject pointerCubePrefab;
    [SerializeField] private GameObject gameController;

    //Touching other scripts
    private MapGenerator mapGenerator;
    private PlayerCubePlacement playerCubePlacement;
    private InventoryHandler inventoryHandler;
    private PlayerController playerController;

    private GameObject pointerCube;
    private MeshRenderer pointerCubeMeshRenderer;

    private void Awake()
    {
        //Touching other scripts
        mapGenerator = gameController.GetComponent<MapGenerator>();
        playerCubePlacement = GetComponent<PlayerCubePlacement>();
        inventoryHandler = GetComponent<InventoryHandler>();
        playerController = GetComponent<PlayerController>();

        pointerCube = Instantiate(pointerCubePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        pointerCubeMeshRenderer = pointerCube.GetComponent<MeshRenderer>();

        //Subscriptions
        inventoryHandler.onActiveSlotChanged += ChangePointerCubeMaterial;
        playerController.onRaycastHitDifferentCube += ChangePointerCubePosition;
    }

    public bool RayCastSequence(float distance, out RaycastHit hit)
    {
        return Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, playerCubePlacement.cubePlacementDistance);
    }

    public Vector3? GetPlayerCubePlacementPosition(RaycastHit hit)
    {
        Transform hitTransform = hit.transform;
        Vector3 hitPoint = hit.point;

        Vector3 delta = (hitPoint - hitTransform.position).Abs();

        DebugManager.Log($"Hitted object name: {hitTransform.name} - {hitTransform.position.GetStringOfVector3()} /// {hitPoint.GetStringOfVector3()}");
        DebugManager.Log($"{delta.GetStringOfVector3()}");

        Vector3 placementLocation = new Vector3(hitTransform.position.x, hitTransform.position.y, hitTransform.position.z);

        bool isXHighest = delta.x > delta.y && delta.x > delta.z;
        bool isYHighest = delta.y > delta.x && delta.y > delta.z;

        if (isXHighest)
        {
            placementLocation.x += GetSideModifier(hitTransform.position.x, hitPoint.x);
        }
        else if (isYHighest)
        {
            placementLocation.y += GetSideModifier(hitTransform.position.y, hitPoint.y);
        }
        else
        {
            placementLocation.z += GetSideModifier(hitTransform.position.z, hitPoint.z);
        }

        return placementLocation;
    }

    public struct RaycastResult
    {
        public Vector3 position;

        public GameObject objectHit;

    }

    public RaycastResult RayCastSequence(float distance)
    {
        RaycastResult raycastResult = new RaycastResult();

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, playerCubePlacement.cubePlacementDistance))
        {
            Transform hitTransform = hit.transform;
            Vector3 hitPoint = hit.point;

            Vector3 delta = (hitPoint - hitTransform.position).Abs();

            DebugManager.Log($"Hitted object name: {hitTransform.name} - {hitTransform.position.GetStringOfVector3()} /// {hitPoint.GetStringOfVector3()}");
            DebugManager.Log($"{delta.GetStringOfVector3()}");

            Vector3 placementLocation = new Vector3(hitTransform.position.x, hitTransform.position.y, hitTransform.position.z);

            bool isXHighest = delta.x > delta.y && delta.x > delta.z;
            bool isYHighest = delta.y > delta.x && delta.y > delta.z;

            if (isXHighest)
            {
                placementLocation.x += GetSideModifier(hitTransform.position.x, hitPoint.x);
            }
            else if (isYHighest)
            {
                placementLocation.y += GetSideModifier(hitTransform.position.y, hitPoint.y);
            }
            else
            {
                placementLocation.z += GetSideModifier(hitTransform.position.z, hitPoint.z);
            }
            raycastResult.position = placementLocation;
            raycastResult.objectHit = hit.collider.gameObject;
        }
        return raycastResult;
    }

    private int GetSideModifier(float hitTransform, float hitPoint)
    {
        if (hitTransform > hitPoint)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }

    private void ChangePointerCubePosition()
    {
        //Vector3? upcomingCubePosition = RayCastSequence();

        //if (upcomingCubePosition == null)
        //{
        //    if (pointerCubeMeshRenderer.enabled)
        //    {
        //        pointerCubeMeshRenderer.enabled = false;
        //    }
        //    return;
        //}
        //else
        //{
        //    pointerCubeMeshRenderer.enabled = true;
        //}

        //pointerCube.transform.position = (Vector3)upcomingCubePosition;

        //DebugManager.Log($"Cube has changed location to {upcomingCubePosition?.GetStringOfVector3()}");
    }

    private void ChangePointerCubeMaterial()
    {
        Material materialOfActiveCube = inventoryHandler.ReturnActiveTransparentCubeMaterial();
        if (materialOfActiveCube != null)
        {
            pointerCubeMeshRenderer.sharedMaterial = materialOfActiveCube;
        }
    }
}
