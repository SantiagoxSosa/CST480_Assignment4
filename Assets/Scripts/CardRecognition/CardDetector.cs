using UnityEngine;
using Vuforia;

/// <summary>
/// Attach this script to every Vuforia Image Target GameObject in your scene.
/// It bridges Vuforia tracking events to ScanPhaseManager.
/// </summary>
public class CardDetector : MonoBehaviour, ITrackableEventHandler
{
    private TrackableBehaviour _trackable;
    private bool _isTracked = false;

    // The name of this Image Target (set automatically from the GameObject name)
    private string TargetName => gameObject.name;

    private void Start()
    {
        _trackable = GetComponent<TrackableBehaviour>();
        if (_trackable != null)
            _trackable.RegisterTrackableEventHandler(this);
        else
            Debug.LogWarning($"CardDetector on '{TargetName}': No TrackableBehaviour found.");
    }

    private void OnDestroy()
    {
        if (_trackable != null)
            _trackable.UnregisterTrackableEventHandler(this);
    }

    // ── Vuforia Callback ──────────────────────────────────────────────────────

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus,
                                        TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnCardFound();
        }
        else
        {
            OnCardLost();
        }
    }

    // ── Tracking Events ───────────────────────────────────────────────────────

    private void OnCardFound()
    {
        if (_isTracked) return; // already reported, don't fire twice
        _isTracked = true;

        Debug.Log($"CardDetector: Found '{TargetName}'");
        ScanPhaseManager.Instance?.OnCardDetected(TargetName);
    }

    private void OnCardLost()
    {
        if (!_isTracked) return;
        _isTracked = false;

        Debug.Log($"CardDetector: Lost '{TargetName}'");
        ScanPhaseManager.Instance?.OnCardLost(TargetName);
    }
}