using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private GameObject unitModel;
    private readonly Color _selectedColor = Color.cadetBlue;
    private Color _defaultColor;
    private Renderer _renderer;

    public Tile CurrentTile { get; set; }

    private void Awake()
    {
        if (unitModel == null) return;
        _renderer = unitModel.GetComponent<Renderer>();
        if (_renderer == null) return;
        unitModel.transform.position = transform.position;
        _defaultColor = _renderer.material.color;
    }

    public Vector3 GetUnitWorldPosition()
    {
        return unitModel == null ? Vector3.zero : unitModel.transform.position;
    }

    public void SetUnitWorldPosition(Vector3 position)
    {
        if (unitModel == null) return;
        unitModel.transform.position = position;
    }

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

    #region MouseEvent

    // here all that happen when u click the unit.
    public void OnPointerDown(PointerEventData eventData)
    {
        /*
         * todo lo que pase al hacer click izq en una unidad.
         */
        if (eventData.button == PointerEventData.InputButton.Left) print("Unit clicked");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    #endregion
}