using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueNodeLibraryAsset", menuName = "Dialogue/DialogueNodeLibraryAsset")]
public class DialogueNodeLibraryAsset : ScriptableObject
{
    public List<DialogueNodeLibraryCategory> categories;

    public DialogueNode GetDialogueByEndId(string categoryName, string endId)
    {
        DialogueNodeLibraryCategory category = categories.Find(category => category.categoryName == categoryName);
        if (category != null)
        {
            return category.dialogues.Find(dialogue => dialogue.EndDialoguesID == endId);
        }
        return null;
    }
}
