using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHUD : MonoBehaviour
{
    [Header("Player")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerHPText;

    [Header("Opponent")]
    public TextMeshProUGUI opponentNameText;
    public TextMeshProUGUI opponentHPText;

    [Header("Battle Log")]
    public TextMeshProUGUI battleMessageText;

    private void OnEnable()
    {
        BattleManager.Instance.OnBattleStart += HandleBattleStart;
        BattleManager.Instance.OnBattleMessage += HandleMessage;
        BattleManager.Instance.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        BattleManager.Instance.OnBattleStart -= HandleBattleStart;
        BattleManager.Instance.OnBattleMessage -= HandleMessage;
        BattleManager.Instance.OnStateChanged -= HandleStateChanged;
    }

    private void HandleBattleStart(PokemonData player, PokemonData opponent)
    {
        UpdatePlayer(player);
        UpdateOpponent(opponent);
    }

    private void HandleMessage(string message)
    {
        battleMessageText.text = message;
    }

    private void HandleStateChanged(BattleState state)
    {
        UpdatePlayer(BattleManager.Instance.playerActivePokemon);
        UpdateOpponent(BattleManager.Instance.opponentActivePokemon);
    }

    private void UpdatePlayer(PokemonData p)
    {
        if (p == null) return;
        playerNameText.text = p.pokemonName;
        playerHPText.text = $"{p.currentHP} / {p.maxHP}";
    }

    private void UpdateOpponent(PokemonData p)
    {
        if (p == null) return;
        opponentNameText.text = p.pokemonName;
        opponentHPText.text = $"{p.currentHP} / {p.maxHP}";
    }
}
