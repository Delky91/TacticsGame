using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public delegate void TileClickedHandler(Tile tile);

    [SerializeField] private bool isObstacle;
    [SerializeField] private int movementCost = 1;

    private bool _isMoveTarget;
    private Transform _transform;

    public bool IsObstacle => isObstacle;
    public int MovementCost => movementCost;

    public Unit OccupyingUnit { get; private set; }
    public bool IsOccupied => OccupyingUnit != null;

    private void Start()
    {
        _transform = GetComponent<Transform>();
    }

    public static event TileClickedHandler OnTileClicked;

    public Vector3 GetTileWorldPosition()
    {
        return transform.position;
    }

    /// <summary>
    ///     Set a unit to the tile
    /// </summary>
    public void SetUnit(Unit unit)
    {
        OccupyingUnit = unit;
    }

    /// <summary>
    ///     Return the unit set to the Tile
    /// </summary>
    public Unit GetUnit()
    {
        return OccupyingUnit;
    }

    /// <summary>
    ///     Highlight the tile as a potential movement target
    /// </summary>
    public void HighlightAsMoveTarget(bool highlight)
    {
        _isMoveTarget = highlight;

        if (highlight)
            HighlightPoolManager.Instance.HighlightTile(this, HighlightPoolManager.HighlightType.Movement);
        else
            HighlightPoolManager.Instance.ReturnToPool(this);
    }

    public void SetAsObstacle(bool isObstacleInRange)
    {
        isObstacle = isObstacleInRange;
    }

    /// <summary>
    ///     Establece el costo de movimiento
    /// </summary>
    public void SetMovementCost(int cost)
    {
        movementCost = Mathf.Max(1, cost); // Asegurarse de que nunca sea menor que 1
    }

    /// <summary>
    ///     Establece si la casilla está ocupada o no
    /// </summary>
    public void SetOccupied(bool occupied)
    {
        if (!occupied)
            OccupyingUnit = null;
    }

    #region MouseEvent

    // here all that happen when u click the tile.
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (IsOccupied)
                // Sí hay una unidad en la casilla, seleccionarla
                OccupyingUnit.SelectUnit();
            else
                // Si la casilla está vacía, notificar que se ha hecho click en ella
                OnTileClicked?.Invoke(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsOccupied || _isMoveTarget) return;

        // Usar el highlight de hover
        HighlightPoolManager.Instance.HighlightTile(this, HighlightPoolManager.HighlightType.Hover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsOccupied) return;

        if (_isMoveTarget)
            // Si es un tile de movimiento, mantener el highlight de movimiento
            HighlightPoolManager.Instance.HighlightTile(this, HighlightPoolManager.HighlightType.Movement);
        else
            // Si no, quitar el highlight de hover
            HighlightPoolManager.Instance.ReturnToPool(this);
    }

    #endregion
}