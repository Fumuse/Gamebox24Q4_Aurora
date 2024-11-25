using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class AudioLibraryCategory
{
    public string categoryName;
    public List<AudioLibraryItem> clips;

    public AudioClip this[string clipLabel]
    {
        get
        {
            AudioLibraryItem assetItem = clips.FirstOrDefault((item) => item.label.Equals(clipLabel));
            if (assetItem == null) return null;
            return assetItem.clip;
        }
    }
}