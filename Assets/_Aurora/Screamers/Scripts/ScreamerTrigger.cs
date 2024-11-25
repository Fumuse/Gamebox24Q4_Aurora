using UnityEngine;

public class ScreamerTrigger : MonoBehaviour, IIlluminated
{
    [SerializeField] private Screamer _screamer;
    [SerializeField] private Transform _positionSpawn;
    
    private bool _isTriggered;

    public bool Illuminate()
    {
        if (_isTriggered) return false;

        if (_screamer)
        {
            _isTriggered = true;
            _screamer.transform.localPosition = _positionSpawn.position;
            _screamer.Activate(true);
            return true;
        }
        
        return false;
    }
}
