using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings", order = 0)]
public class GameSettings : ScriptableObject
{
    [SerializeField] private int timeToEnd = 240;
    [SerializeField] private int maxAcceptance = 100;

    public int TimeToEnd => timeToEnd;
    public int MaxAcceptance => maxAcceptance;
}