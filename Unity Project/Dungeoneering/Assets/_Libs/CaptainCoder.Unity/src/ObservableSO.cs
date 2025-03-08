using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObservableSO : ScriptableObject
{
    /// <summary>
    /// In the Editor, this will be called during PlayModeStateChange.EnteredPlayMode. In a build, this will be called during OnEnable.
    /// </summary>
    public virtual void OnEnterPlayMode()
    {

    }

    /// <summary>
    /// In the Editor, this will be called during PlayModeStateChange.ExitPlaymode. In a build, this will be called during OnDisable
    /// </summary>
    protected virtual void OnExitPlayMode()
    {

    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnPlayModeStateChange;
#else
        OnEnterPlayMode();
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
#else
        OnExitPlayMode();
#endif
    }

#if UNITY_EDITOR
    private void OnPlayModeStateChange(PlayModeStateChange change)
    {
        switch (change)
        {
            case PlayModeStateChange.EnteredPlayMode:
                OnEnterPlayMode();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                OnExitPlayMode();
                break;
        }
    }
#endif


}