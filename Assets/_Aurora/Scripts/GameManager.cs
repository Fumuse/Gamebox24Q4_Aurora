using System;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
    [SerializeField] private GameSettings settings;
    [SerializeField] private bool startInTutorial = true;
    [SerializeField] private HouseStageEnum houseStage = HouseStageEnum.Light;
    [SerializeField] private House house;
    
    [Header("Скрипты для инициализации")]
    [SerializeField] private GameProvidersManager providersManager;
    [SerializeField] private PlayerStateMachine player;
    [SerializeField] private TutorialStateMachine tutorial;
    [SerializeField] private InputReader inputReader;

    public GameSettings Settings => settings;

    #region GameStages
    private bool _tutorialStage = false;
    public bool TutorialStage
    {
        get => _tutorialStage;
        private set => _tutorialStage = value;
    }
    #endregion

    #region Options
    public bool TimeSpentWhenTeleport => !TutorialStage;

    public HouseStageEnum CurrentStage
    {
        get => houseStage;
        set
        {
            houseStage = value;
            ChangeHouseSprites();
        }
    }

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

        Init();
    }

    /// <summary>
    /// Единая точка входа
    /// </summary>
    private void Init()
    {
        Timer = new Timer(Settings.TimeToEnd);
        AcceptanceScale = new AcceptanceScale(Settings.MaxAcceptance);
        TagManager = new TagManager();

        inputReader.Init();
        providersManager.Init();
        InitObjects();
    }

    private void InitObjects()
    {
        foreach(Room room in house.Rooms)
        {
            foreach (InteractableObject interObject in room.InteractableObjects)
            {
                interObject.Init();
            }
            foreach (Door door in room.Doors)
            {
                door.Init();
            }
        }
    }

    private void OnValidate()
    {
        if (settings == null)
        {
            Debug.LogError("You need to attach the game settings to game manager!");
        }

        house ??= FindFirstObjectByType<House>();
        player ??= FindFirstObjectByType<PlayerStateMachine>();
        tutorial ??= FindFirstObjectByType<TutorialStateMachine>();
        providersManager ??= FindFirstObjectByType<GameProvidersManager>();
        inputReader ??= FindFirstObjectByType<InputReader>();
    }

    public void InitPlayer()
    {
        player.Init();
    }

    private void Start()
    {
        ChangeHouseSprites();
        StartTutorial();
    }

    private void StartTutorial()
    {
        if (!startInTutorial)
        {
            InitPlayer();
            return;
        }
        TutorialStage = true;
        
        tutorial.StartTutorial();
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