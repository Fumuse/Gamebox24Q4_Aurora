using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameSettings settings;
    [SerializeField] private bool startInTutorial = true;
    [SerializeField] private HouseStageEnum houseStage = HouseStageEnum.Light;
    [SerializeField] private House house;
    [SerializeField] private LayerMask interactableObjectLayerMask;

    [Header("Скрипты для инициализации")] 
    [SerializeField] private CleanupEvents cleanupEvents;
    [SerializeField] private GameProvidersManager providersManager;
    [SerializeField] private PlayerStateMachine player;
    [SerializeField] private TutorialStateMachine tutorial;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Flashlight flashlight;
    [SerializeField] private MouseHoverDetector mouseHoverDetector;
    [SerializeField] private UnconditionalInformationHandler unconditionalInformationHandler;

    public static Action OnTutorialStateChanged;

    public GameSettings Settings => settings;

    #region GameStages
    private bool _tutorialStage = false;
    public bool TutorialStage
    {
        get => _tutorialStage;
        private set
        {
            _tutorialStage = value;
            OnTutorialStateChanged?.Invoke();
        }
    }
    #endregion

    #region Options
    public bool ScalesSpentWhenTutorial => !TutorialStage;

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
    
    public CleanupEvents CleanupEvents { get; private set; }
    
    public AmbienceController AmbienceController { get; private set; }

    public LayerMask InteractableObjectLayerMask => interactableObjectLayerMask;

    public PlayerStateMachine Player => player;

    protected override void Awake()
    {
        base.Awake();

        Init();
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
        cleanupEvents ??= FindAnyObjectByType<CleanupEvents>();
        mouseHoverDetector ??= FindAnyObjectByType<MouseHoverDetector>();
        unconditionalInformationHandler ??= FindAnyObjectByType<UnconditionalInformationHandler>();
        flashlight ??= FindAnyObjectByType<Flashlight>();
    }

    private void Start()
    {
        ChangeHouseSprites();
        StartTutorial();
    }

    private void OnEnable()
    {
        TagManager.OnTagAdded += OnTagAdded;
    }

    private void OnDisable()
    {
        TagManager.OnTagAdded -= OnTagAdded;
    }

    #region Initing project

    /// <summary>
    /// Единая точка входа
    /// </summary>
    private void Init()
    {
        InitSettings();

        CleanupEvents = cleanupEvents;
        CleanupEvents.Init();
        
        inputReader.Init();
        providersManager.Init();
        
        Timer = new Timer(Settings.TimeToEnd);
        AcceptanceScale = new AcceptanceScale(Settings.MaxAcceptance);
        TagManager = new TagManager();
        
        flashlight.Init();

        unconditionalInformationHandler.Init();
        AmbienceController = new AmbienceController();
        InitObjects();
        mouseHoverDetector.Init();
    }

    private void InitSettings()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 36;
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
            
            room.Shadow.Init();
        }
    }

    public void InitPlayer()
    {
        player.Init();
    }

    #endregion

    #region Tutorial
    private void StartTutorial()
    {
        if (!startInTutorial)
        {
            AmbienceAudioController.Instance.StartPlayBackgroundMusic();
            TagManager.AddTag(new Tag(TagEnum.TutorialEnded));
            InitPlayer();
            return;
        }
        TutorialStage = true;
        
        tutorial.StartTutorial();
    }

    public void EndTutorial()
    {
        TutorialStage = false;
        tutorial.EndTutorial();
    }
    #endregion

    public void ChangeHouseSprites()
    {
        foreach(Room room in house.Rooms)
        {
            room.ChangeSpriteStage(houseStage);
        }

        player.ChangePlayerSpriteByStage(houseStage);

        AmbienceAudioController.Instance.ChangeBackgroundMusicByStage(houseStage);
    }

    private void OnTagAdded()
    {
        PhotoAlbumManage();
    }

    private void PhotoAlbumManage()
    {
        Tag gramophoneTag = new Tag(TagEnum.GramophoneTaken);
        if (!TagManager.HasTag(gramophoneTag)) return;
        
        Tag twigTag = new Tag(TagEnum.WalkingStickTaken);
        if (!TagManager.HasTag(twigTag)) return;
        
        Tag bearTag = new Tag(TagEnum.BearTaken);
        if (!TagManager.HasTag(bearTag)) return;

        foreach (Room room in house.Rooms)
        {
            foreach (InteractableObject interObject in room.InteractableObjects)
            {
                if (interObject.CompareTag("Loc_2_PhotoAlbum"))
                {
                    TagManager.AddTag(new Tag(TagEnum.PhotoAlbumIsAssembled));
                    
                    interObject.enabled = false;
                    interObject.gameObject.SetActive(false);
                    break;
                }
            }
        }
    }
}