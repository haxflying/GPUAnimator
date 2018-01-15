using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class ZBaker
{
    private GameObject bakeGo;
    private Mesh bakeMesh;
    private IList<AnimationClip> clips;
    public IList<AnimationClip> TryGetDataFromGameObject(GameObject go)
    {
        bakeGo = go;
        clips = new List<AnimationClip>();
        if(go.GetComponent<Animation>())
        {
            Animation anim = go.GetComponent<Animation>();
            foreach(AnimationState state in anim)
            {
                clips.Add(state.clip);
            }
        }
        else if(go.GetComponent<Animator>())
        {
            Animator anim = go.GetComponent<Animator>();
            clips = new List<AnimationClip>(anim.runtimeAnimatorController.animationClips);
        }

        return clips;
    }
   

    public AnimClipInfo Bake(List<AnimationClip> clipList, List<AnimStates> stateList)
    {
        bakeMesh = new Mesh();
        AnimClipInfo ClipDict = ScriptableObject.CreateInstance<AnimClipInfo>();

        for (int c = 0; c < clipList.Count; c++)
        {
            AnimationClip clip = clipList[c];
            float sampleTime = 0f;
            float frameTime = 0f;
            int clipframe = 0;

            clipframe = Mathf.ClosestPowerOfTwo((int)(clip.frameRate * clip.length));
            frameTime = clip.length / clipframe;            

            //Init texture
            bakeGo.GetComponentInChildren<SkinnedMeshRenderer>().BakeMesh(bakeMesh);
            int width = Mathf.ClosestPowerOfTwo(bakeMesh.vertexCount);
            Texture2D animTex = new Texture2D(width, clipframe, TextureFormat.RGBAHalf, false);
            animTex.name = bakeGo.name + "_" + clip.name;

            for (int i = 0; i < clipframe; i++)
            {
                clip.SampleAnimation(bakeGo, sampleTime);
                bakeGo.GetComponentInChildren<SkinnedMeshRenderer>().BakeMesh(bakeMesh);
                //TODO set pixel
                for (int j = 0; j < bakeMesh.vertexCount; j++)
                {
                    Vector3 vertex = bakeMesh.vertices[j];
                    animTex.SetPixel(j, i, new Color(vertex.x, vertex.y, vertex.z));
                }

                //Debug.Log(bakeMesh.vertices[0].x);
                sampleTime += frameTime;
            }
            animTex.Apply();
            ClipData clipData = new ClipData()
            {
                texture = animTex,
                length = clip.length
            };
            ClipDict.clips.Add(stateList[c], clipData);
        }

        return ClipDict;   
    }
}
