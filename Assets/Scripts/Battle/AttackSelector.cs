using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackSelector : MonoBehaviour
{
    [Header("Attack Buttons (assign 4 in Inspector)")]
    public Button[] attackButtons;

    private void OnEnable()
    {
        BattleManager.Instance.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        BattleManager.Instance.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(BattleState state)
    {
        bool isPlayerTurn = state == BattleState.PlayerTurn;
        gameObject.SetActive(isPlayerTurn);

        if (!isPlayerTurn) return;

        PokemonData active = BattleManager.Instance.playerActivePokemon;

        for (int i = 0; i < attackButtons.Length; i++)
        {
            bool hasAttack = i < active.attacks.Length;
            attackButtons[i].gameObject.SetActive(hasAttack);

            if (hasAttack)
            {
                int index = i; // capture for lambda
                attackButtons[i].GetComponentInChildren<TextMeshProUGUI>().text =
                    $"{active.attacks[i].attackName} ({active.attacks[i].damage} dmg)";
                attackButtons[i].onClick.RemoveAllListeners();
                attackButtons[i].onClick.AddListener(() => BattleManager.Instance.PlayerAttack(index));
            }
        }
    }
}