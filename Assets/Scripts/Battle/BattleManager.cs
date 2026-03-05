using UnityEngine;
using System.Collections.Generic;

public enum BattleState { Start, PlayerTurn, OpponentTurn, Won, Lost }

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("Player & Opponent Pokemon")]
    public List<PokemonData> playerTeam = new List<PokemonData>();
    public List<PokemonData> opponentTeam = new List<PokemonData>();

    [Header("Active Pokemon")]
    public PokemonData playerActivePokemon;
    public PokemonData opponentActivePokemon;

    [Header("State")]
    public BattleState currentState;

    // Events other scripts can listen to
    public System.Action<PokemonData, PokemonData> OnBattleStart;
    public System.Action<string> OnBattleMessage;
    public System.Action<BattleState> OnStateChanged;
    public System.Action OnBattleEnd;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Call this once both teams are loaded from AR card detection
    public void StartBattle()
    {
        if (playerTeam.Count == 0 || opponentTeam.Count == 0)
        {
            Debug.LogWarning("BattleManager: One or both teams are empty.");
            return;
        }

        playerActivePokemon = playerTeam[0];
        opponentActivePokemon = opponentTeam[0];

        playerActivePokemon.ResetHP();
        opponentActivePokemon.ResetHP();

        SetState(BattleState.PlayerTurn);
        OnBattleStart?.Invoke(playerActivePokemon, opponentActivePokemon);
        OnBattleMessage?.Invoke($"Battle Start! {playerActivePokemon.pokemonName} vs {opponentActivePokemon.pokemonName}");
    }

    // Called by AttackSelector when the player picks an attack
    public void PlayerAttack(int attackIndex)
    {
        if (currentState != BattleState.PlayerTurn) return;

        Attack attack = playerActivePokemon.attacks[attackIndex];
        ApplyDamage(attack, playerActivePokemon, opponentActivePokemon);
        OnBattleMessage?.Invoke($"{playerActivePokemon.pokemonName} used {attack.attackName} for {attack.damage} damage!");

        if (CheckFainted(opponentActivePokemon, opponentTeam))
            return; // CheckFainted handles next steps

        SetState(BattleState.OpponentTurn);
        Invoke(nameof(OpponentTakeTurn), 1.5f); // Small delay for feel
    }

    private void OpponentTakeTurn()
    {
        if (opponentActivePokemon.attacks.Length == 0) return;

        // Simple AI: pick a random attack
        int index = Random.Range(0, opponentActivePokemon.attacks.Length);
        Attack attack = opponentActivePokemon.attacks[index];
        ApplyDamage(attack, opponentActivePokemon, playerActivePokemon);
        OnBattleMessage?.Invoke($"{opponentActivePokemon.pokemonName} used {attack.attackName} for {attack.damage} damage!");

        if (CheckFainted(playerActivePokemon, playerTeam))
            return;

        SetState(BattleState.PlayerTurn);
    }

    private void ApplyDamage(Attack attack, PokemonData attacker, PokemonData defender)
    {
        int damage = attack.damage;

        // Apply weakness (x2) and resistance (-30), standard TCG rules
        if (attack.damage > 0)
        {
            if (attacker.type == defender.weakness) damage *= 2;
            if (attacker.type == defender.resistance) damage = Mathf.Max(0, damage - 30);
        }

        defender.TakeDamage(damage);
    }

    // Returns true if battle is over, false if it continues
    private bool CheckFainted(PokemonData fainted, List<PokemonData> team)
    {
        if (!fainted.isFainted) return false;

        OnBattleMessage?.Invoke($"{fainted.pokemonName} fainted!");
        team.Remove(fainted);

        if (team.Count == 0)
        {
            bool playerLost = (team == playerTeam);
            SetState(playerLost ? BattleState.Lost : BattleState.Won);
            OnBattleMessage?.Invoke(playerLost ? "You lost!" : "You won!");
            OnBattleEnd?.Invoke();
            return true;
        }

        // Auto-send next Pokemon
        if (team == playerTeam)
            playerActivePokemon = team[0];
        else
            opponentActivePokemon = team[0];

        return false;
    }

    private void SetState(BattleState newState)
    {
        currentState = newState;
        OnStateChanged?.Invoke(newState);
    }
}
