using UnityEngine;
using Random = UnityEngine.Random;

public class ScreamerSpawner : MonoBehaviour
{
    [SerializeField] private TeleportProvider teleportProvider;
    [SerializeField] private Shadow[] screamersToSpawn;
    [SerializeField, Range(1, 99)] private int chanceToSpawnScreamer = 30;
    
    private void OnValidate()
    {
        teleportProvider ??= FindAnyObjectByType<TeleportProvider>();
    }

    private void OnEnable()
    {
        teleportProvider.OnPlayerTeleported += OnPlayerTeleported;
    }

    private void OnDisable()
    {
        teleportProvider.OnPlayerTeleported -= OnPlayerTeleported;
    }

    private void OnPlayerTeleported()
    {
        if (GameManager.Instance.AcceptanceScale.Current > GameManager.Instance.Settings.AcceptanceToScreamerSpawn) return;

        foreach (Shadow shadow in screamersToSpawn)
        {
            if (Random.Range(0, 100) > chanceToSpawnScreamer) continue;
            shadow.Spawn();
        }
    }
}