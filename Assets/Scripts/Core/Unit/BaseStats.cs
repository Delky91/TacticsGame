using UnityEngine;

[CreateAssetMenu(fileName = "BaseStats", menuName = "Base Unit Stats", order = 0)]
public class BaseStats : ScriptableObject
{
    public int health;
    public int movementPoints;
}