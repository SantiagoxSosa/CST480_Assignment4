using UnityEngine;
using Vuforia;

public class CardDetector : MonoBehaviour
{
    private ObserverBehaviour _observer;

    private void Start()
    {
        _observer = GetComponent<ObserverBehaviour>();
        if (_observer != null)
        {
            _observer.OnTargetStatusChanged += OnTargetStatusChanged;
        } else
        {
            Debug.Log("No Observer");
        }
    }

    private void OnDestroy()
    {
        if (_observer != null)
            _observer.OnTargetStatusChanged -= OnTargetStatusChanged;
    }

    private void OnTargetStatusChanged(ObserverBehaviour observer, TargetStatus status)
    {
        bool found = status.Status == Status.TRACKED ||
                     status.Status == Status.EXTENDED_TRACKED;

        if (found)
        {
            Debug.Log("Card found, sending to scan phase");
            ScanPhaseManager.Instance?.OnCardDetected(gameObject.name);
        }
        else
            ScanPhaseManager.Instance?.OnCardLost(gameObject.name);
    }
}