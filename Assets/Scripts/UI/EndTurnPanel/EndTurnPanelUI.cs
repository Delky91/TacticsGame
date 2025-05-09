using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button button;

    private void Start()
    {
        button.onClick.AddListener(HandleButtonClick);
        UpdateUI();


        TurnSystem.Instance.OnPlayerTurnStarted += UpdateUI;
        TurnSystem.Instance.OnEnemyTurnStarted += UpdateUI;
        TurnSystem.Instance.OnPreparationPhase += UpdateUI;
        TurnSystem.Instance.OnCombatStarted += UpdateUI;
    }

    private void OnDestroy()
    {
        if (!TurnSystem.Instance) return;

        TurnSystem.Instance.OnPlayerTurnStarted -= UpdateUI;
        TurnSystem.Instance.OnEnemyTurnStarted -= UpdateUI;
        TurnSystem.Instance.OnPreparationPhase -= UpdateUI;
        TurnSystem.Instance.OnCombatStarted -= UpdateUI;
    }

    private void HandleButtonClick()
    {
        if (TurnSystem.Instance.IsPreparationPhase)
            TurnSystem.Instance.StartCombat();
        else
            TurnSystem.Instance.NextTurn();
    }


    private void UpdateUI()
    {
        if (TurnSystem.Instance.IsPreparationPhase)
        {
            text.text = "Start Combat";
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
        }
        else
        {
            text.text = TurnSystem.Instance.IsPlayerTurn ? "Player's turn" : "Enemy's turn";
            button.GetComponentInChildren<TextMeshProUGUI>().text = "End Turn";
        }
    }
}