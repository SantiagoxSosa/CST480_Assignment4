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
        Debug.Log("State changes, trying to give attack options");
        if (attackButtons == null)
        {
            Debug.LogError("CRASH PREVENTED: 'attackButtons' array is null! Set its size in the Inspector.");
            return;
        }
        bool isPlayerTurn = state == BattleState.PlayerTurn;
        gameObject.SetActive(isPlayerTurn);

        if (!isPlayerTurn) return;

        PokemonData active = BattleManager.Instance.playerActivePokemon;
        if (active == null) {
            Debug.Log("Null active pokemon");
            return;
        }

        if (active.attacks == null)
        {
            Debug.LogError($"CRASH PREVENTED: The 'attacks' array on {active.pokemonName} is null! Check the ScriptableObject.");
            return;
        }

        for (int i = 0; i < attackButtons.Length; i++)
        {
            if (attackButtons[i] == null)
            {
                Debug.LogWarning($"AttackSelector: Button slot {i} is empty in the Inspector!");
                continue;
            }

            bool hasAttack = i < active.attacks.Length;
            Debug.Log("Amount of attacks" + active.attacks.Length);
            attackButtons[i].gameObject.SetActive(hasAttack);

            if (hasAttack)
            {
                Debug.Log("Has Attack " + i);
                int index = i; // capture for lambda
                var textComp = attackButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (textComp != null)
                {
                    textComp.text = $"{active.attacks[i].attackName} ({active.attacks[i].damage} dmg)";
                } else
                {
                    Debug.LogError($"AttackSelector: Button {i} is missing a TextMeshProUGUI child!");
                }
                attackButtons[i].onClick.RemoveAllListeners();
                attackButtons[i].onClick.AddListener(() => BattleManager.Instance.PlayerAttack(index));
            }
        }
    }
}