using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AmbienceAnimation : MonoBehaviour
{
    [SerializeField] private HouseStageEnum[] showInStages;
    [SerializeField] private bool showOnce = false;

    public HouseStageEnum[] ShowInStages => showInStages;

    private bool _isShowed = false;
    public bool CanShowed => !(showOnce && _isShowed);

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.enabled = false;
    }

    private void Start()
    {
        if (!ShowInStages.Contains(GameManager.Instance.CurrentStage))
        {
            Hide();
        }
    }

    public void Show()
    {
        if (!CanShowed) return;

        ShowSprite();

        _animator.enabled = true;
        _animator.Play(
            _animator.GetCurrentAnimatorStateInfo(0).shortNameHash, -1, 0f
        );
        
        _isShowed = true;
    }

    public void ShowSprite()
    {
        if (!ShowInStages.Contains(GameManager.Instance.CurrentStage)) return;
        
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
