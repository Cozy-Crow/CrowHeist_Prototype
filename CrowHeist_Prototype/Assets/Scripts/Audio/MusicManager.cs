using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Unity.VisualScripting;
using UnityEngine.AI;
using System;
using System.IO;


public class MusicManager : MonoBehaviour
{
    #region Serialized Fields
    public static MusicManager Instance;
    [SerializeField] private string currentMusicName;
    [SerializeField]
    private EventReference
    menuTheme,
    crowleyTheme,
    abuelitaLevel,

    abuelitaIntro,
    abuelitaOutro;


    [SerializeField] private bool canSetParameter = false;

    #region FMOD Parameters
    [SerializeField] private float trinketsCollected = 0;
    [SerializeField] private int itemYes = 0;
    [SerializeField] private int reutrnLocation = 0;

    #endregion

    #endregion

    #region Properties
    public EventInstance CurrentMusicInstance { get; private set; }
    public string ActiveMusicName { get => currentMusicName; set => currentMusicName = value; }
    public EventReference MenuTheme { get => menuTheme; }
    public EventReference CrowleyTheme { get => crowleyTheme; }
    public EventReference AbuelitaLevel { get => abuelitaLevel; }

    #endregion
    #region Music Manager
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Reset()
    {
        Instance.CurrentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        ActiveMusicName = string.Empty;
    }

    // Get or set the current music instance given its name
    void Update()
    {
        if (ActiveMusicName.Equals("AbuelitaLevel"))
        {
            CurrentMusicInstance.getParameterByName("trinketsCollected", out float trinketsCollected);
            CurrentMusicInstance.getParameterByName("itemYes", out float itemYes);
            CurrentMusicInstance.getParameterByName("returnLocation", out float reutrnLocation);
            trinketsCollected = (int)trinketsCollected;
            itemYes = (int)itemYes;
            reutrnLocation = (int)reutrnLocation;

            if (!canSetParameter)
                return;

            if (MusicManager.Instance != null)
            {
                SetParameterByName("trinketsCollected", trinketsCollected);
                SetParameterByName("itemYes", itemYes);
                SetParameterByName("returnLocation", reutrnLocation); 
            }
            

        }
    }

    public bool WillChangeTo(EventReference newMusic)
    {
        if (!CurrentMusicInstance.isValid())
            return true;

        EventDescription eventDescription = RuntimeManager.GetEventDescription(newMusic);
        eventDescription.getPath(out string newMusicPath);
        
        Debug.Log(newMusicPath);
        Debug.Log(ActiveMusicName);
        return !ActiveMusicName.Equals(newMusicPath, StringComparison.OrdinalIgnoreCase);
    }


    public void PlayMusic(EventReference music, bool fadeout = false, float fadeTime = 2f)
    {
        EventDescription eventDescription = RuntimeManager.GetEventDescription(music);
        eventDescription.getPath(out string newMusicPath);

        if (CurrentMusicInstance.isValid() && ActiveMusicName.Equals(newMusicPath, StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log($"MusicManager: " + newMusicPath + " is already playing");
            return;
        }

        if (!CurrentMusicInstance.isValid())
        {
            CurrentMusicInstance = RuntimeManager.CreateInstance(music);
            CurrentMusicInstance.start();

            ActiveMusicName = newMusicPath;
            return;
        }

        if (!fadeout)
        {
            CurrentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            CurrentMusicInstance = RuntimeManager.CreateInstance(music);
            CurrentMusicInstance.start();
            ActiveMusicName = newMusicPath;
        }
        else
        {
            StartCoroutine(FadeMusicOutAndIn(fadeTime, music));
        }

        print("Playing music: " + Instance.ActiveMusicName);
    }



    public void StopMusic()
    {
        try
        {
            CurrentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            ActiveMusicName = string.Empty;
        }
        catch (SystemException)
        {
            Debug.LogWarning("MusicManager: StopMusic failed to stop " + Instance.CurrentMusicInstance);
        }
    }

    public static void SetParameterByName(string parameter, float value)
    {
        try
        {
            Debug.Log("Setting " + parameter + " to " + value);
            Instance.CurrentMusicInstance.setParameterByName(parameter, value);
            Debug.Log(Instance.CurrentMusicInstance.getParameterByName(parameter, out float paraValue));
        }
        catch (System.Exception)
        {
            Debug.LogWarning("MusicManager: SetParameter failed to set " + parameter + "to " + value);
        }
    }
    public static void SetParameterByLabel(string parameter, string label)
    {
        Instance.CurrentMusicInstance.setParameterByNameWithLabel(parameter, label);
    }

    private IEnumerator FadeMusicOutAndIn(float delay, EventReference newMusic)
    {
        CurrentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        yield return new WaitForSeconds(delay);

        string newMusicName = GetEventName(newMusic);
        if (ActiveMusicName.Equals(newMusicName, StringComparison.OrdinalIgnoreCase))
            yield break;

        CurrentMusicInstance = RuntimeManager.CreateInstance(newMusic);
        CurrentMusicInstance.start();

        //#if UNITY_EDITOR
        //ActiveMusicName = GetMusicName(newMusic);
        //#endif
    }

    public static string GetEventName(string path)
    {
        if (string.IsNullOrEmpty(path))
            return "";
        int lastSlash = path.LastIndexOf('/');
        return lastSlash >= 0 ? path.Substring(lastSlash + 1) : path;
    }

    public static string GetEventName(EventReference music)
    {
        if (string.IsNullOrEmpty(music.ToString()))
            return string.Empty;

        return GetEventName(music.ToString());
    }
}

/*#if UNITY_EDITOR
    private static string GetMusicName(EventReference music)
    {
        if (string.IsNullOrEmpty(music.Path))
            return string.Empty;

        return Path.GetFileName(music.Path);
    }
#endif
}
#endregion
#if UNITY_EDITOR

[CustomTimelineEditor(typeof(MusicManager))]
public class MusicMangerEditor : Editor
{
    SerializedProperty
    CanSetParameter,
    TrinketsCollected,
    ItemYes,
    ReturnLocation;

    bool AdaptiveGroup;

    private void OnEnable()
    {
        CanSetParameter = serializedObject.FindProperty("canSetParameter");
        TrinketsCollected = serializedObject.FindProperty("trinketsCollected");
        ItemYes = serializedObject.FindProperty("itemYes");
        ReturnLocation = serializedObject.FindProperty("reutrnLocation");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        MusicManager musicManager = (MusicManager)target;
        EditorGUILayout.PropertyField(CanSetParameter);
        if (musicManager.ActiveMusicName.Equals("AbuelitaLevel"))
        {
            if (AdaptiveGroup = EditorGUILayout.BeginFoldoutHeaderGroup(AdaptiveGroup, "FMOD Parameters"))
            {
                ShowDateParameters();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        serializedObject.ApplyModifiedProperties();

    }
        private void OnDisable()
    {
        serializedObject.ApplyModifiedProperties();
    }

    private void ShowDateParameters()
    {
        EditorGUILayout.LabelField("Date Parameters", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(TrinketsCollected);
        EditorGUILayout.PropertyField(ItemYes);
        EditorGUILayout.PropertyField(ReturnLocation);
    }
}    
 #endif
 #endregion
*/
#endregion