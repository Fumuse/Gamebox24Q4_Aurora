using UnityEngine;

public class SreamerTrigger : MonoBehaviour
{
    [SerializeField] private Screamer _screamer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_screamer)
        {
            if (collision.TryGetComponent(out Flashlight flashlight))
            {
                _screamer.Activate(true);
            }
        }
    }
}
