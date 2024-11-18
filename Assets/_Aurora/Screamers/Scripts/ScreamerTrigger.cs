using UnityEngine;

public class ScreamerTrigger : MonoBehaviour
{
    [SerializeField] private Screamer _screamer;
    [SerializeField] private Transform _positionSpawn;
    private bool _isTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isTrigger) return;

        if (_screamer) 
        {
            if (collision.TryGetComponent(out FlashlightTEST flashlight)) //поменять на Flashlight
            {
                _isTrigger = true;
                _screamer.transform.position = _positionSpawn.position;
                _screamer.Activate(true);
            }
        }
    }
}
