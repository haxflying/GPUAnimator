using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "clipInfos", menuName = "AnimInfoDict")]
public class AnimClipInfo : SerializedScriptableObject {

    public Dictionary<AnimStates, ClipData> clips = new Dictionary<AnimStates, ClipData>();
}

public enum AnimStates
{
    idle,
    attack01,
    attack02,
    gethit,
    victory,
    walk,
    death
}

[SerializeField]
public class ClipData
{
    public Texture2D texture;
    public float length;
}
