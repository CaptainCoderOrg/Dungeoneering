using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObservableSO : ScriptableObject
{
    protected virtual void OnEnterPlayMode()
    {

    }

    protected virtual void OnEnterEditMode()
    {

    }

    protected virtual void OnExitEditMode()
    {

    }

    protected virtual void OnExitPlayMode()
    {

    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnPlayModeStateChange;
#endif
        AfterEnabled();
    }

    public virtual void AfterEnabled()
    {
        // Debug.Log($"SO Enabled: {this}");
    }

#if UNITY_EDITOR
    private void OnPlayModeStateChange(PlayModeStateChange change)
    {
        switch (change)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnEnterEditMode();
                break;
            case PlayModeStateChange.ExitingEditMode:
                OnExitEditMode();
                break;
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