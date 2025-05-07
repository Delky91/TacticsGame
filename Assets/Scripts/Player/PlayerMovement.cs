using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /*
     * Pasos del sistema de movimiento del player:
     * 1) saber cuantos PM tiene el player por default (crear scriptable Object con estadísticas). ok
     * 2) crear variable para mantener los PM que tiene actualmente. (por buff y debuff). ok
     * 3) Pm máximos actuales para mantener después del final de turno.
     * 4) método para gastar PM.
     * 5) método para que al finalizar el turno los pm vuelvan a su valor maximo actual.
     * 6) Eventos para el clic tanto en selección como para mover a un tile.
     * 7) Generar una utilities para el highlight de los tiles a los cuales es permitido moverse.
     */

    // get the base stats / TODO: cambiar por sistema de selección de personaje.
    [SerializeField] private BaseStats baseStats;
    private int _currentMovementPoints; // crear un evento más adelante para mostrar los cambien en UI
    private int _maxMovementPoints;

    private void Start()
    {
        // In the beginning the _current is = to the basePM
        _currentMovementPoints = baseStats.movementPoints;
    }

    /// <summary>
    ///     Method to use movement point.
    /// </summary>
    /// <param name="pointsUsed">Int</param>
    public void UsePm(int pointsUsed)
    {
        if (pointsUsed < 0) return; // pointUsed only can be a positive number
        if (_currentMovementPoints < pointsUsed) return; // if the player doesn't have enough pm
        if (_currentMovementPoints < 0) _currentMovementPoints = 0; // no negatives pm
        _currentMovementPoints -= pointsUsed;
    }

    public int GetCurrentPm()
    {
        return _currentMovementPoints;
    }

    public int GetMaxPm()
    {
        return _maxMovementPoints;
    }
}