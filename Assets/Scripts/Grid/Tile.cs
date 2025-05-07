using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
///     Represents a single tile in the game grid system, managing unit placement and position information.
///     This component handles the occupation state and provides methods to manipulate units on the tile.
/// </summary>
public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private const float ColorLerpSpeed = 10f;

    // Color on hover:
    private readonly Color _hoverColor = Color.softYellow;
    private Color _defaultColor;
    private Renderer _renderer;
    private Transform _transform;
    public Unit OccupyingUnit { get; private set; }
    public bool IsOccupied => OccupyingUnit != null;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _transform = GetComponent<Transform>();
        if (_renderer == null || _transform == null)
        {
            Debug.LogError("No se encontró el tilePrefab");
            return;
        }

        _defaultColor = _renderer.material.color;
    }

    public Vector3 GetTileWorldPosition(Tile tile)
    {
        return tile == null ? Vector3.zero : tile.transform.position;
    }

    /// <summary>
    ///     Set a unit to the tile
    /// </summary>
    /// <param name="unit"></param>
    public void SetUnit(Unit unit)
    {
        OccupyingUnit = unit;
    }

    /// <summary>
    ///     Return the unit set to the Tile
    /// </summary>
    /// <returns>Unit</returns>
    public Unit GetUnit()
    {
        return OccupyingUnit;
    }

    /// <summary>
    ///     Clear the tile of any Unit
    /// </summary>
    public void RemoveUnitFromTile()
    {
        OccupyingUnit = null;
    }

    #region MouseEvent

    // here all that happen when u click the tile.
    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsOccupied) return;
        if (eventData.button == PointerEventData.InputButton.Left)
            print("llamado a GetTileWorldPosition" + GetTileWorldPosition(GetComponent<Tile>()));
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

    #endregion
}