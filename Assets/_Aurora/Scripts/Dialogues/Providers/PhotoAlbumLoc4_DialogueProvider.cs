using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PhotoAlbumLoc4_DialogueProvider : IChangeDialogueProvider
{
    private DialogueView _dialogueView;
    private SpriteLibraryAsset _spriteLibrary;
    private DialogueNodeLibraryAsset _dialogueLibrary;
    private TagManager _tagManager;

    private const string SpriteCategory = "Acceptance";
    private const string DialogueCategory = "Acceptance";
    
    private int DialoguePoints { get; set; } = 0;

    public static Action OnDeath;
    public static Action InEndDialogue;
    public static Action<int> EndDialoguePoints;

    public PhotoAlbumLoc4_DialogueProvider(DialogueView view, SpriteLibraryAsset spriteLibrary, DialogueNodeLibraryAsset dialogueAssets)
    {
        _dialogueView = view;
        _spriteLibrary = spriteLibrary;
        _dialogueLibrary = dialogueAssets;
        _tagManager = GameManager.Instance.TagManager;

        AmbienceAudioController.Instance.ChangeBackgroundMusicByEndDialogue(EndGamePath.Acceptance);
        
        InEndDialogue?.Invoke();
    }
    
    public void ChangeDialogue(ref DialogueNode dialogueNode, ActionSettings actionSettings)
    {
        //Если у нас 4 очка - сразу кат-сцена смерть
        if (DialoguePoints >= 4)
        {
            OnDeath?.Invoke();
            GameProvidersManager.Instance.DialogueProvider.Cancel();
            return;
        }
        
        if (dialogueNode.EndDialoguesID == "Loc_4_PhotoAlbum_EndDialogue_1")
        {
            if (!_tagManager.HasTag(new Tag(TagEnum.CandleTaken)))
            {
                DialoguePoints += 1;
            }
        }
        else if (
            dialogueNode.EndDialoguesID == "Loc_4_PhotoAlbum_EndDialogue_1_Reaction_1" ||
            dialogueNode.EndDialoguesID == "Loc_4_PhotoAlbum_EndDialogue_1_Reaction_3" ||
            
            dialogueNode.EndDialoguesID == "Loc_4_PhotoAlbum_EndDialogue_2_Reaction_2" ||
            dialogueNode.EndDialoguesID == "Loc_4_PhotoAlbum_EndDialogue_2_Reaction_4" ||
            
            dialogueNode.EndDialoguesID == "Loc_4_PhotoAlbum_EndDialogue_3_Reaction_2" ||
            dialogueNode.EndDialoguesID == "Loc_4_PhotoAlbum_EndDialogue_3_Reaction_3"
        )
        {
            DialoguePoints += 1;
        }
        else if (
            dialogueNode.EndDialoguesID == "Loc_4_PhotoAlbum_EndDialogue_2_Reaction_3" ||
            dialogueNode.EndDialoguesID == "Loc_4_PhotoAlbum_EndDialogue_3_Reaction_4"
        )
        {
            DialoguePoints += 2;
        }
        else if (dialogueNode.EndDialoguesID == "Loc_4_PhotoAlbum_EndDialogue_Final")
        {
            dialogueNode = GetEndDialogueByPoints();
        }
        
        _dialogueView.ChangeSomeImageAction(GetSpriteByPoints());
    }
    
    private Sprite GetSpriteByPoints()
    {
        if (DialoguePoints >= 3)
        {
            return _spriteLibrary.GetSprite(SpriteCategory, "Acceptance_4");
        }
        
        if (DialoguePoints >= 2)
        {
            return _spriteLibrary.GetSprite(SpriteCategory, "Acceptance_3");
        }
        
        if (DialoguePoints >= 1)
        {
            return _spriteLibrary.GetSprite(SpriteCategory, "Acceptance_2");
        }
        
        return _spriteLibrary.GetSprite(SpriteCategory, "Acceptance_1");
    }

    private DialogueNode GetEndDialogueByPoints()
    {
        EndDialoguePoints?.Invoke(DialoguePoints);
        
        if (DialoguePoints >= 3)
        {
            return _dialogueLibrary.GetDialogueByEndId(DialogueCategory, "Loc_4_PhotoAlbum_EndDialogue_Final_4");
        }

        if (DialoguePoints >= 2)
        {
            return _dialogueLibrary.GetDialogueByEndId(DialogueCategory, "Loc_4_PhotoAlbum_EndDialogue_Final_3");
        }

        if (DialoguePoints >= 1)
        {
            return _dialogueLibrary.GetDialogueByEndId(DialogueCategory, "Loc_4_PhotoAlbum_EndDialogue_Final_2");
        }

        return _dialogueLibrary.GetDialogueByEndId(DialogueCategory, "Loc_4_PhotoAlbum_EndDialogue_Final_1");
    }
}