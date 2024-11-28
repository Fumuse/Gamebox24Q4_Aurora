using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioLibraryAsset", menuName = "Audio/AudioLibraryAsset")]
public class AudioLibraryAsset : ScriptableObject
{
    [SerializeField] private List<AudioLibraryCategory> categories;
    
    public AudioClip this[string categoryName, string clipLabel]
    {
        get
        {
            AudioLibraryCategory category =
                categories.FirstOrDefault((category) => category.categoryName.Equals(categoryName));
            if (category == null) return null;
            return category[clipLabel];
        }
    }
}