using UnityEngine;
using UnityEngine.InputSystem;

public class SelectSystem : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private Unit _selectedUnit;

    private void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        var ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out var hit)) return;

        if (hit.collider.TryGetComponent(out Unit unit))
        {
            _selectedUnit = unit;
#if UNITY_EDITOR
            Debug.Log("unit selected");
#endif
        }
        else if (hit.collider.TryGetComponent(out Tile tile))
        {
#if UNITY_EDITOR
            Debug.Log($"tile selected in position: {tile.GetTileWorldPosition(tile)}");
#endif
        }
    }
}