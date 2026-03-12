using TMPro;
using UnityEngine;

public class ScanUIIndicator : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI statusText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ScanPhaseManager.Instance != null)
        {
            // Unsubscribe first to avoid double-subscriptions
            ScanPhaseManager.Instance.OnScanMessage -= UpdateStatusText;
            ScanPhaseManager.Instance.OnScanMessage += UpdateStatusText;

            // Manually trigger the initial message so it's not blank
            UpdateStatusText(ScanPhaseManager.Instance.currentPhase == ScanPhase.Player1Scanning
                ? "Player 1: Show your 3 Pokémon cards"
                : "Waiting for scan...");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        // Subscribe to the scan message event
        if (ScanPhaseManager.Instance != null)
        {
            ScanPhaseManager.Instance.OnScanMessage += UpdateStatusText;
        }
    }

    private void OnDisable()
    {
        // Always unsubscribe to prevent memory leaks or errors
        if (ScanPhaseManager.Instance != null)
        {
            ScanPhaseManager.Instance.OnScanMessage -= UpdateStatusText;
        }
    }

    private void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
            Debug.Log($"UI Display: {message}");
        }
    }
}
