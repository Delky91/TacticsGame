using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
///     Represents a single tile in the game grid system, managing unit placement and position information.
///     This component handles the occupation state and provides methods to manipulate units on the tile.
/// </summary>
public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Color on hover:
    private readonly Color _hoverColor = Color.softYellow;
    private Color _defaultColor;
    private Unit _occupyingUnit;
    private Renderer _renderer;
    public bool IsOccupied => _occupyingUnit != null;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogError("No se encontró Renderer en el tilePrefab");
            return;
        }

        _defaultColor = _renderer.material.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsOccupied || _renderer == null) return;
        _renderer.material.color = _hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsOccupied || _renderer == null) return;
        _renderer.material.color = _defaultColor;
    }

    public Vector3 GetTileWorldPosition(Tile tile)
    {
        return tile == null ? Vector3.zero : tile.transform.position;
    }

    public void SetUnit(Unit unit)
    {
        _occupyingUnit = unit;
    }

    public Unit GetUnit()
    {
        return _occupyingUnit;
    }

    public void ClearUnit()
    {
        _occupyingUnit = null;
    }
}