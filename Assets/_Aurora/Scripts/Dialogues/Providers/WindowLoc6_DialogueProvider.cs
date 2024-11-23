using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class WindowLoc6_DialogueProvider : IChangeDialogueProvider
{
    private DialogueView _dialogueView;
    private SpriteLibraryAsset _spriteLibrary;
    private DialogueNodeLibraryAsset _dialogueLibrary;
    private TagManager _tagManager;

    private const string SpriteCategory = "Escape";
    private const string DialogueCategory = "Escape";
    private int DialoguePoints { get; set; } = 0;

    public static Action OnDeath;

    public WindowLoc6_DialogueProvider(DialogueView view, SpriteLibraryAsset spriteLibrary, DialogueNodeLibraryAsset dialogueAssets)
    {
        _dialogueView = view;
        _spriteLibrary = spriteLibrary;
        _dialogueLibrary = dialogueAssets;
        _tagManager = GameManager.Instance.TagManager;
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
        
        if (dialogueNode.EndDialoguesID == "Room_6_Window_EndDialogue_1")
        {
            if (!_tagManager.HasTag(new Tag(TagEnum.TVOn)))
            {
                DialoguePoints += 1;
            }
        }
        else if (
            dialogueNode.EndDialoguesID == "Room_6_Window_EndDialogue_1_Reaction_1" ||
            dialogueNode.EndDialoguesID == "Room_6_Window_EndDialogue_1_Reaction_3" ||
            
            dialogueNode.EndDialoguesID == "Room_6_Window_EndDialogue_2_Reaction_2" ||
            dialogueNode.EndDialoguesID == "Room_6_Window_EndDialogue_2_Reaction_4" ||
            
            dialogueNode.EndDialoguesID == "Room_6_Window_EndDialogue_3_Reaction_1" ||
            dialogueNode.EndDialoguesID == "Room_6_Window_EndDialogue_3_Reaction_4"
        )
        {
            DialoguePoints += 1;
        }
        else if (
            dialogueNode.EndDialoguesID == "Room_6_Window_EndDialogue_2_Reaction_3" ||
            dialogueNode.EndDialoguesID == "Room_6_Window_EndDialogue_3_Reaction_2"
        )
        {
            DialoguePoints += 2;
        }
        else if (dialogueNode.EndDialoguesID == "Room_6_Window_EndDialogue_Final")
        {
            dialogueNode = GetEndDialogueByPoints();
        }
        
        _dialogueView.ChangeSomeImageAction(GetSpriteByPoints());
    }

    private Sprite GetSpriteByPoints()
    {
        if (DialoguePoints >= 3)
        {
            return _spriteLibrary.GetSprite(SpriteCategory, "Escape_4");
        }
        
        if (DialoguePoints >= 2)
        {
            return _spriteLibrary.GetSprite(SpriteCategory, "Escape_3");
        }
        
        if (DialoguePoints >= 1)
        {
            return _spriteLibrary.GetSprite(SpriteCategory, "Escape_2");
        }
        
        return _spriteLibrary.GetSprite(SpriteCategory, "Escape_1");
    }

    private DialogueNode GetEndDialogueByPoints()
    {
        if (DialoguePoints >= 3)
        {
            return _dialogueLibrary.GetDialogueByEndId(DialogueCategory, "Room_6_Window_EndDialogue_Final_4");
        }

        if (DialoguePoints >= 2)
        {
            return _dialogueLibrary.GetDialogueByEndId(DialogueCategory, "Room_6_Window_EndDialogue_Final_3");
        }

        if (DialoguePoints >= 1)
        {
            return _dialogueLibrary.GetDialogueByEndId(DialogueCategory, "Room_6_Window_EndDialogue_Final_2");
        }

        return _dialogueLibrary.GetDialogueByEndId(DialogueCategory, "Room_6_Window_EndDialogue_Final_1");
    }
}