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

	[MenuItem("Window/ZGPUAnimator")]
    static void ShowWindow()
    {
        GetWindow<ZGPUAnimatorWindow>("GPU Animator");
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
    private static AnimMapBaker baker;
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
        GUI.skin.label.fontSize = 15;
        GUI.skin.label.alignment = TextAnchor.UpperCenter;
        GUILayout.Label("AnimationClip to bake");

        EditorGUILayout.ObjectField("Current Bake Object", currentGameobject, typeof(GameObject), true);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(_assetListProperty, true);
        EditorGUILayout.PropertyField(_assetStateProperty, true);
        EditorGUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
            //sync 2 list
            while(_assetState.Count < _assetList.Count)
            {
                _assetState.Add(AnimStates.idle);
            }
            while(_assetState.Count > _assetList.Count)
            {
                _assetState.RemoveAt(_assetState.Count - 1);
            }
        }

        GUILayout.Space(10);
        GUILayout.Label("Path to Save");
        EditorGUILayout.LabelField(string.Format("Save at :  {0}", Path.Combine(path, subPath)));
        EditorGUILayout.BeginHorizontal();
        path = EditorGUILayout.TextField(path);       
        subPath = EditorGUILayout.TextField(subPath);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUI.skin.button.fontSize = 15;
        if(GUILayout.Button("Bake"))
        {
            if (_assetList.Count == 0)
            {
                EditorUtility.DisplayDialog("Warning", "List is Empty", "OK");
                return;
            }         

            if(baker == null)
            {
                baker = new AnimMapBaker();
            }
           
        }
    }

    IEnumerator bakeAndSave()
    {
        baker.SetAnimData(currentGameobject);
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < _assetList.Count; i++)
        {
            //baker.SetAnimData(_assetList[i]);
            //baker.BakePerAnimClip(_assetList[i].SampleAnimation)
        }
    }
}
