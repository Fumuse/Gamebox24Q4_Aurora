using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings", order = 0)]
public class GameSettings : ScriptableObject
{
    [SerializeField] private int timeToEnd = 240;
    [SerializeField] private int maxAcceptance = 100;
    [SerializeField, Range(0, 255)] private float maxRoomShadowOverlayOpacity = 180f;
    [SerializeField, Range(0, 255)] private float stepRoomShadowOverlayOpacity = 40f;
    [SerializeField] private int acceptanceFlashlightCost = 1;
    [SerializeField] private int acceptanceToBrokenStage = 50;
    [SerializeField] private int acceptanceToScreamerSpawn = 75;
    [SerializeField] private int acceptanceBadEndStep = 25;

    public int TimeToEnd => timeToEnd;
    public int MaxAcceptance => maxAcceptance;
    public float MaxRoomShadowOverlayOpacity => maxRoomShadowOverlayOpacity;
    public float StepRoomShadowOverlayOpacity => stepRoomShadowOverlayOpacity;
    public int AcceptanceFlashlightCost => acceptanceFlashlightCost;
    public int AcceptanceToBrokenStage => acceptanceToBrokenStage;
    public int AcceptanceToScreamerSpawn => acceptanceToScreamerSpawn;
    public int AcceptanceBadEndStep => acceptanceBadEndStep;
}