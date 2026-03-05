using UnityEngine;
using System.Collections.Generic;
using Vuforia;

public enum ScanPhase { Player1Scanning, Player2Scanning, Confirmed }

public class ScanPhaseManager : MonoBehaviour
{
    public static ScanPhaseManager Instance { get; private set; }

    [Header("References")]
    public CardDatabase cardDatabase;
    public BattleManager battleManager;

    [Header("Settings")]
    private const int MAX_TEAM_SIZE = 3;

    // Current scan state
    public ScanPhase currentPhase { get; private set; }
    public List<PokemonData> player1Team { get; private set; } = new List<PokemonData>();
    public List<PokemonData> player2Team { get; private set; } = new List<PokemonData>();

    // Events for UI to listen to
    public System.Action<ScanPhase> OnPhaseChanged;
    public System.Action<PokemonData, int> OnCardScanned;       // card, playerNumber
    public System.Action<string> OnScanMessage;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        StartPlayer1Scan();
    }

    // ── Phase Control ─────────────────────────────────────────────────────────

    public void StartPlayer1Scan()
    {
        player1Team.Clear();
        SetPhase(ScanPhase.Player1Scanning);
        OnScanMessage?.Invoke("Player 1: Show your 3 Pokémon cards");
    }

    public void StartPlayer2Scan()
    {
        player2Team.Clear();
        SetPhase(ScanPhase.Player2Scanning);
        OnScanMessage?.Invoke("Player 2: Show your 3 Pokémon cards");
    }

    public void ConfirmTeams()
    {
        if (player1Team.Count < MAX_TEAM_SIZE || player2Team.Count < MAX_TEAM_SIZE)
        {
            OnScanMessage?.Invoke("Both players need 3 cards scanned before confirming.");
            return;
        }

        SetPhase(ScanPhase.Confirmed);

        // Pass teams to BattleManager and start
        battleManager.playerTeam = new List<PokemonData>(player1Team);
        battleManager.opponentTeam = new List<PokemonData>(player2Team);
        battleManager.StartBattle();
    }

    // ── Called by CardDetector when Vuforia finds an image target ─────────────

    public void OnCardDetected(string targetName)
    {
        PokemonData pokemon = cardDatabase.GetByName(targetName);

        if (pokemon == null)
        {
            Debug.LogWarning($"ScanPhaseManager: No PokemonData found for target '{targetName}'");
            return;
        }

        if (currentPhase == ScanPhase.Player1Scanning)
            TryAddToTeam(pokemon, player1Team, 1);
        else if (currentPhase == ScanPhase.Player2Scanning)
            TryAddToTeam(pokemon, player2Team, 2);
    }

    public void OnCardLost(string targetName)
    {
        // Cards are locked once scanned — losing tracking doesn't remove them
        // Visual feedback only, handled by CardDetector
    }

    // ── Private Helpers ───────────────────────────────────────────────────────

    private void TryAddToTeam(PokemonData pokemon, List<PokemonData> team, int playerNumber)
    {
        // Don't add duplicates
        if (team.Contains(pokemon))
        {
            OnScanMessage?.Invoke($"{pokemon.pokemonName} is already on Player {playerNumber}'s team.");
            return;
        }

        // Don't exceed max team size
        if (team.Count >= MAX_TEAM_SIZE)
        {
            OnScanMessage?.Invoke($"Player {playerNumber} already has {MAX_TEAM_SIZE} cards locked in.");
            return;
        }

        team.Add(pokemon);
        OnCardScanned?.Invoke(pokemon, playerNumber);
        OnScanMessage?.Invoke($"Player {playerNumber}: {pokemon.pokemonName} added! ({team.Count}/{MAX_TEAM_SIZE})");

        // Auto-advance phase when Player 1 fills their team
        if (playerNumber == 1 && team.Count == MAX_TEAM_SIZE)
        {
            OnScanMessage?.Invoke("Player 1 team locked! Player 2, show your cards.");
            Invoke(nameof(StartPlayer2Scan), 1.5f);
        }

        // Auto-prompt confirm when Player 2 fills their team
        if (playerNumber == 2 && team.Count == MAX_TEAM_SIZE)
        {
            OnScanMessage?.Invoke("Player 2 team locked! Press Confirm to start the battle.");
        }
    }

    private void SetPhase(ScanPhase newPhase)
    {
        currentPhase = newPhase;
        OnPhaseChanged?.Invoke(newPhase);
    }
}
