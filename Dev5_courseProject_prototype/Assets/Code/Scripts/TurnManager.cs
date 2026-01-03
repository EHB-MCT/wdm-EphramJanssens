using System;
using Unity.VisualScripting;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance {get; private set;}
    public event Action<UnitTeam> OnTurnChanged;

    [field: SerializeField] public UnitTeam CurrentTurn {get; private set;} = UnitTeam.Player;
    [SerializeField] private int turnCounter = 1;
    private float turnStartTime;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartTurn(UnitTeam.Player);
    }

    public void EndTurn()
    {
        float duration = Time.time - turnStartTime;

        if (GameLogger.Instance != null)
        {
            GameLogger.Instance.LogAction("TurnEnd", new
            {
                team = CurrentTurn.ToString(),
                round = turnCounter,
                durationSeconds = duration
            });
        }

        if (CurrentTurn == UnitTeam.Player)
        {
            StartTurn(UnitTeam.Enemy);
        }
        else
        {
            StartTurn(UnitTeam.Player);
            turnCounter++;
        }
    }

    private void StartTurn(UnitTeam team)
    {
        turnStartTime = Time.time;

        CurrentTurn = team;
        Debug.Log($"Start turn: {team} (Round {turnCounter})");

        Unit[] allUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);
        foreach (Unit unit in allUnits)
        {
            if (unit.Team == team)
            {
                unit.ResetTurn();
            }
        }

        OnTurnChanged?.Invoke(CurrentTurn);

        if (team == UnitTeam.Enemy)
        {
            Invoke(nameof(EndTurn), 1.5f);
        }
    }

    public bool IsMyTurn(UnitTeam team)
    {
        return team == CurrentTurn;
    }
}
