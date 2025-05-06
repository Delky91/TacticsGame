using System;
using UnityEngine;

/// <summary>
///     Manages the turn-based system for player and enemy actions.
///     Implements the Singleton pattern to ensure only one instance exists.
/// </summary>
public class TurnSystem : MonoBehaviour
{
    /// <summary>
    ///     Singleton instance of the TurnSystem.
    /// </summary>
    public static TurnSystem Instance { get; private set; }

    /// <summary>
    ///     Indicates whether it's currently the player's turn.
    /// </summary>
    public bool IsPlayerTurn { get; private set; }

    /// <summary>
    ///     Indicates whether the game is in the preparation phase.
    /// </summary>
    public bool IsPreparationPhase { get; private set; } = true;

    private bool IsCombatStarted { get; set; }

    /// <summary>
    ///     Initializes the singleton instance. If an instance already exists,
    ///     the current object will be destroyed.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        PreparationPhase();
    }

    private void PreparationPhase()
    {
        IsPreparationPhase = true;
        IsCombatStarted = false;
        OnPreparationPhase?.Invoke();
    }

    public void StartCombat()
    {
        if (IsCombatStarted) return;

        IsCombatStarted = true;
        IsPreparationPhase = false;
        IsPlayerTurn = true;
        OnCombatStarted?.Invoke();
        OnPlayerTurnStarted?.Invoke();
    }


    /// <summary>
    ///     Switches the turn between player and enemy, triggering appropriate events.
    /// </summary>
    public void NextTurn()
    {
        IsPlayerTurn = !IsPlayerTurn;

        if (IsPlayerTurn)
        {
            IsPreparationPhase = true;
            OnPreparationPhase?.Invoke();
            IsPreparationPhase = false;
            OnPlayerTurnStarted?.Invoke();
        }
        else
        {
            OnEnemyTurnStarted?.Invoke();
        }
    }

    public event Action OnPlayerTurnStarted;
    public event Action OnEnemyTurnStarted;
    public event Action OnPreparationPhase;
    public event Action OnCombatStarted;
}