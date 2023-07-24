using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCubePlacement : MonoBehaviour
{
    [SerializeField] private GameObject cubeInHand;
    [SerializeField] private LayerMask gridLayerMask;
    [SerializeField] private float raycastDistance = 100.0f;

    void Start()
    {
        
    }

    void Update()
    {
        PlaceBlock();
    }

    private void PlaceBlock()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, raycastDistance, gridLayerMask))
            {
                Vector3 targetPosition = GetNearestGridPosition(hit.collider.gameObject.transform.position);

                Instantiate(cubeInHand, targetPosition, Quaternion.identity);
            }
        }
    }

    Vector3 GetNearestGridPosition(Vector3 position) 
    {
        float x = position.x;
        float y = position.y;
        float z = position.z;

        Vector3 targetPosition = new Vector3(x, y+2.0f, z);

        return targetPosition;
    }
}
