using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
    [SerializeField] private GameSettings settings;
    [SerializeField] private bool tutorialStage = false;
    [SerializeField] private HouseStageEnum houseStage = HouseStageEnum.Light;
    [SerializeField] private House house;
    [SerializeField] private PlayerStateMachine player;

    public GameSettings Settings => settings;

    #region GameStages
    public bool TutorialStage
    {
        get => tutorialStage;
        set
        {
            tutorialStage = value;
            
            //TODO: Выполнение каких-то событий
        }
    }
    #endregion

    #region Options
    public bool TimeSpentWhenTeleport => !TutorialStage;
    public HouseStageEnum CurrentStage => houseStage;

    public HouseStageEnum OppositeSpriteStage
    {
        get
        {
            if (CurrentStage == HouseStageEnum.Light)
                return HouseStageEnum.Dark;
            
            return HouseStageEnum.Light;
        }
    }
    #endregion

    #region Scales
    public Timer Timer { get; private set; }
    public AcceptanceScale AcceptanceScale { get; private set; }
    #endregion

    public TagManager TagManager { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();

        Timer = new Timer(Settings.TimeToEnd);
        AcceptanceScale = new AcceptanceScale(Settings.MaxAcceptance);
        TagManager = new TagManager();
    }

    private void OnValidate()
    {
        if (settings == null)
        {
            Debug.LogError("You need to attach the game settings to game manager!");
        }

        house ??= FindFirstObjectByType<House>();
        player ??= FindFirstObjectByType<PlayerStateMachine>();
    }

    private void Start()
    {
        ChangeHouseSprites();
    }

    public void ChangeHouseSprites()
    {
        foreach(Room room in house.Rooms)
        {
            room.ChangeSpriteStage(houseStage);
        }

        player.ChangePlayerSpriteByStage(houseStage);
    }
}