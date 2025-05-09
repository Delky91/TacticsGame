using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    // Delegate for unit selection
    public delegate void UnitSelectedHandler(Unit unit);


    [SerializeField] private GameObject unitModel;
    private readonly Color _selectedColor = Color.cadetBlue;
    private Color _defaultColor;
    private Renderer _renderer;
    public Factions Faction { get; private set; }
    public Tile CurrentTile { get; set; }


    private void Awake()
    {
        if (!unitModel) return; // add logic to throw error
        _renderer = unitModel.GetComponent<Renderer>();
        if (!_renderer) return; // add logic to throw error
        unitModel.transform.position = transform.position;
        _defaultColor = _renderer.material.color;
    }

    //Event for unit selection
    public static event UnitSelectedHandler OnUnitSelected;


    public Vector3 GetUnitWorldPosition()
    {
        return !unitModel ? Vector3.zero : unitModel.transform.position;
    }

    public void SetUnitWorldPosition(Vector3 position)
    {
        if (!unitModel) return;
        unitModel.transform.position = position;
    }

    public void AssignFaction(Factions faction)
    {
        Faction = faction;
    }

    #region MouseEvent

    // here all that happen when u click the unit.
    public void OnPointerDown(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                SelectUnit();
                break;
            case PointerEventData.InputButton.Right:
                print("Unit right clicked");
                break;
            case PointerEventData.InputButton.Middle:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        // show stats in the UI
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    #endregion

    #region Select / Deselect Unit

    public void SelectUnit()
    {
        if (_renderer) _renderer.material.color = _selectedColor;
        OnUnitSelected?.Invoke(this);
    }

    public void DeselectUnit()
    {
        if (_renderer) _renderer.material.color = _defaultColor;
    }

    #endregion

    #region Active / Deactive gameObject

    // can be used in case that the unit die in combat
    public void DesactivateModelUnit()
    {
        unitModel.SetActive(false);
        //here can place death animations, sounds.
    }

    public void ActivateModelUnit()
    {
        unitModel.SetActive(true);
    }

    #endregion
}