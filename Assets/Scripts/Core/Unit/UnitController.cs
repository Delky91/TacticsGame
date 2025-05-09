using UnityEngine;

public class UnitController : MonoBehaviour
{
    private Unit _selectedUnit;
    private PlayerMovement _selectedUnitMovement;

    private void Start()
    {
        Unit.OnUnitSelected += HandleUnitSelected;
        Tile.OnTileClicked += HandleTileClicked;
    }

    private void OnDestroy()
    {
        Unit.OnUnitSelected -= HandleUnitSelected;
        Tile.OnTileClicked -= HandleTileClicked;
    }

    private void HandleUnitSelected(Unit unit)
    {
        // Si ya hay una unidad seleccionada, deseleccionar
        if (_selectedUnit && _selectedUnit != unit) _selectedUnit.DeselectUnit();

        _selectedUnit = unit;
        _selectedUnitMovement = unit.GetComponent<PlayerMovement>();
    }

    private void HandleTileClicked(Tile tile)
    {
        // Si hay una unidad seleccionada y se hace clic en una casilla, intentar mover la unidad
        if (_selectedUnit && _selectedUnitMovement) _selectedUnitMovement.MoveToTile(tile);
    }
}