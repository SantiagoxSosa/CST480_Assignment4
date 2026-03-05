using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnIndicator : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI turnText;
    public Image indicatorImage;

    [Header("Colors")]
    public Color playerColor = Color.blue;
    public Color opponentColor = Color.red;

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
        switch (state)
        {
            case BattleState.PlayerTurn:
                turnText.text = "Your Turn";
                indicatorImage.color = playerColor;
                break;
            case BattleState.OpponentTurn:
                turnText.text = "Opponent's Turn";
                indicatorImage.color = opponentColor;
                break;
            case BattleState.Won:
                turnText.text = "You Win!";
                indicatorImage.color = playerColor;
                break;
            case BattleState.Lost:
                turnText.text = "You Lose!";
                indicatorImage.color = opponentColor;
                break;
        }
    }
}
