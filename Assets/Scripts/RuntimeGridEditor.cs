/*****************************************************************************
// File Name : RuntimeGridEditor.cs
// Author : Arcadia Koederitz
// Creation Date : 8/20/2026
// Last Modified : 8/20/2026
//
// Brief Description : Debug script to edit the gridmap during runtime.
*****************************************************************************/
using Gridmap;
using UnityEngine;
using UnityEngine.InputSystem;

public class RuntimeGridEditor : MonoBehaviour
{
    [SerializeField] private GridTileBase gridTile;

    private InputAction placeAction;
    private InputAction removeAction;

    private void Awake()
    {
        if (TryGetComponent(out PlayerInput pi))
        {
            placeAction = pi.currentActionMap.FindAction("Place");
            removeAction = pi.currentActionMap.FindAction("Remove");

            placeAction.started += Handle_PlaceStarted;
            removeAction.started += Handle_RemoveStarted;
        }
    }

    private void OnDestroy()
    {
        placeAction.started -= Handle_PlaceStarted;
        removeAction.started -= Handle_RemoveStarted;
    }

    private void Handle_PlaceStarted(InputAction.CallbackContext obj)
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 900, LayerMask.GetMask("Default")))
        {
            Gridmap.Gridmap gmap = hit.collider.GetComponentInParent<Gridmap.Gridmap>();
            Vector3 editPos = hit.point + (hit.normal / 2);

            gmap.PlaceTileAtPoint(gridTile, gmap.WorldToGridPosition(editPos));
        }
    }

    private void Handle_RemoveStarted(InputAction.CallbackContext obj)
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 900, LayerMask.GetMask("Default")))
        {
            Gridmap.Gridmap gmap = hit.collider.GetComponentInParent<Gridmap.Gridmap>();
            Vector3 editPos = hit.point - (hit.normal / 2);

            gmap.PlaceTileAtPoint(null, gmap.WorldToGridPosition(editPos));
        }
    }
}
