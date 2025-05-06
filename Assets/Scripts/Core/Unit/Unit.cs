using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private GameObject unitModel;

    private void Awake()
    {
        if (unitModel == null) return;
        unitModel.transform.position = transform.position;
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
}