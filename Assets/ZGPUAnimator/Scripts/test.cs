using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class test : MonoBehaviour {
    [Range(0, 2)]
    public float timeScale;
    [Range(0f, 0.1f)]
    public float blendTime = 0.1f;

    [Range(0, 6)]
    [InlineButton("Play")]
    public int clipIndex;    

    private Material mat;
    private float zTime;
    private MaterialPropertyBlock props;
    private void Play()
    {
        StartCoroutine(SwitchNext((AnimStates)clipIndex, false));
    }

    private void Start()
    {
        zTime = 0f;
        props = new MaterialPropertyBlock();
        mat = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        zTime += Time.deltaTime * timeScale;
        mat.SetFloat("_ZTime", zTime);
        mat.SetFloat("_BlendTime", blendTime);
    }

    public IEnumerator SwitchNext(AnimStates state, bool isLoop = true)
    {
        Texture2D AnimMap = Source.instance.infos.clips[state].texture;
        if (AnimMap == null)
        {
            Debug.LogError("AnimMap is null");
        }

        if(isLoop)
        {
            AnimMap.wrapMode = TextureWrapMode.Repeat;
        }
        else
        {
            AnimMap.wrapMode = TextureWrapMode.Clamp;
        }

        mat.SetTexture("_AnimMapNext", AnimMap);
        mat.SetFloat("_SwitchTime", zTime);
        mat.EnableKeyword("_SWITCHING");

        float length = Source.instance.infos.clips[state].length;
        yield return new WaitForSeconds(length);
        zTime = length;
        mat.DisableKeyword("_SWITCHING");
        mat.SetTexture("_AnimMap", AnimMap);
    }
}
