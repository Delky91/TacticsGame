using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
///     Represents a single tile in the game grid system, managing unit placement and position information.
///     This component handles the occupation state and provides methods to manipulate units on the tile.
/// </summary>
public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    // Evento para cuando se hace click en la casilla
    public delegate void TileClickedHandler(Tile tile);

    private const float ColorLerpSpeed = 10f;

    [SerializeField] private bool isObstacle;
    [SerializeField] private int movementCost = 1; // Por defecto, cuesta 1 PM atravesar


    // Color on hover:
    private readonly Color _hoverColor = Color.softYellow;

    // Color for movement target:
    private readonly Color _moveTargetColor = new(0.0f, 0.8f, 0.0f, 0.6f); // Verde semitransparente
    private Color _defaultColor;
    private bool _isMoveTarget;
    private Renderer _renderer;
    private Transform _transform;

    public bool IsObstacle => isObstacle;
    public int MovementCost => movementCost;

    public Unit OccupyingUnit { get; private set; }
    public bool IsOccupied => OccupyingUnit != null;


    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _transform = GetComponent<Transform>();
        if (_renderer == null || _transform == null) return;

        _defaultColor = _renderer.material.color;
    }

    public static event TileClickedHandler OnTileClicked;

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
    ///     Highlight the tile as a potential movement target
    /// </summary>
    public void HighlightAsMoveTarget(bool highlight)
    {
        if (!_renderer) return;

        _isMoveTarget = highlight;

        _renderer.material.color = highlight ? _moveTargetColor : _defaultColor;
    }

    public void SetAsObstacle(bool isObstacleInRange)
    {
        isObstacle = isObstacleInRange;

        // Opcional: cambiar la apariencia visual si es un obstáculo
        if (_renderer)
            // Un color oscuro para representar obstáculos
            _renderer.material.color = isObstacleInRange ? new Color(0.2f, 0.2f, 0.2f) : _defaultColor;
    }

// Método para establecer el costo de movimiento en tiempo de ejecución
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
            {
                // Sí hay una unidad en la casilla, seleccionarla
                OccupyingUnit.SelectUnit();
            }
            else
            {
                // Si la casilla está vacía, notificar que se ha hecho click en ella
                print("llamado a GetTileWorldPosition" + GetTileWorldPosition(GetComponent<Tile>()));
                OnTileClicked?.Invoke(this);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsOccupied || _renderer == null || _isMoveTarget) return;
        _renderer.material.color = _hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsOccupied || _renderer == null) return;

        if (_isMoveTarget)
            _renderer.material.color = _moveTargetColor;
        else
            _renderer.material.color = _defaultColor;
    }

    #endregion
}