using UnityEngine;
using System.Collections;

public class Source : MonoBehaviour
{

    public static Source instance;

    public AnimClipInfo infos;
    private void Awake()
    {
        instance = this;
    }
}
