using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public enum State
{
    A,C,V,D
}
public class ZGPUAnimatorWindow : EditorWindow {

    private static ZGPUAnimatorWindow myWindow;

	[MenuItem("Window/ZGPUAnimator")]
    static void ShowWindow()
    {
        myWindow = GetWindow<ZGPUAnimatorWindow>("GPU Animator");
    }

    private GameObject currentGameobject;

    [SerializeField]
    protected List<UnityEngine.AnimationClip> _assetList = new List<AnimationClip>();
    [SerializeField]
    protected List<AnimStates> _assetState = new List<AnimStates>();

    protected SerializedObject _serializedObject;
    protected SerializedProperty _assetListProperty, _assetStateProperty;

    private static string path = "DefaultPath";
    private static string subPath = "SubPath";
    private static ZBaker baker;
    protected void OnEnable()
    {
        _serializedObject = new SerializedObject(this);
        _assetListProperty = _serializedObject.FindProperty("_assetList");
        _assetStateProperty = _serializedObject.FindProperty("_assetState");
    }

    private void OnGUI()
    {
        _serializedObject.Update();

        GUILayout.Space(10);

        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 15;
        labelStyle.fontStyle = FontStyle.BoldAndItalic;
        labelStyle.normal.textColor = Color.gray;
        labelStyle.alignment = TextAnchor.UpperCenter;
        GUILayout.Label("AnimationClip to bake", labelStyle);
         GUILayout.Space(10);

        EditorGUI.BeginChangeCheck();

        currentGameobject = (GameObject)EditorGUILayout.ObjectField("Current Bake Object", currentGameobject, typeof(GameObject), true);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(_assetListProperty, true);
        EditorGUILayout.PropertyField(_assetStateProperty, true);
        EditorGUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
        {                      
            _serializedObject.ApplyModifiedProperties();

            if (baker == null)
            {
                baker = new ZBaker();
            }

            IList<AnimationClip> clips = baker.TryGetDataFromGameObject(currentGameobject);
            foreach(var clip in clips)
            {
                if (!_assetList.Contains(clip))
                    _assetList.Add(clip);
            }

            //sync 2 list
            while(_assetState.Count < _assetList.Count)
            {
                _assetState.Add(AnimStates.idle);
            }
            while(_assetState.Count > _assetList.Count)
            {
                _assetState.RemoveAt(_assetState.Count - 1);
            }

            myWindow.ShowNotification(new GUIContent("Data Inited"));
            

            
        }

        GUILayout.Space(10);
        GUILayout.Label("Path to Save", labelStyle);
        EditorGUILayout.LabelField(string.Format("Save at :  {0}", Path.Combine(path, subPath)));
        EditorGUILayout.BeginHorizontal();
        path = EditorGUILayout.TextField(path);       
        subPath = EditorGUILayout.TextField(subPath);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        if(GUILayout.Button("Bake"))
        {
            if (_assetList.Count == 0)
            {
                EditorUtility.DisplayDialog("Warning", "List is Empty", "OK");
                return;
            }


            AnimClipInfo ClipDict = baker.Bake(_assetList, _assetState);
            if (!System.IO.Directory.Exists(Path.Combine("Assets/", path)))
                System.IO.Directory.CreateDirectory(Path.Combine("Assets/", path));

            foreach(var data in ClipDict.clips)
            {
                AssetDatabase.CreateAsset(data.Value.texture, Path.Combine("Assets/", Path.Combine(path, data.Key.ToString() + ".asset")));
            }

            AssetDatabase.CreateAsset(ClipDict, Path.Combine("Assets/", Path.Combine(path, subPath + ".asset")));


            myWindow.ShowNotification(new GUIContent("Bake Complete"));            
        }
    }

    IEnumerator bakeAndSave()
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < _assetList.Count; i++)
        {
            //baker.SetAnimData(_assetList[i]);
            //baker.BakePerAnimClip(_assetList[i].SampleAnimation)
        }
    }
}
