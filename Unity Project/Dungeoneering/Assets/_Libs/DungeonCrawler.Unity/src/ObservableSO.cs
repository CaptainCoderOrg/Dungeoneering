using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObservableSO : ScriptableObject
{
    protected virtual void OnEnterPlayMode()
    {

    }

    protected virtual void OnExitPlayMode()
    {

    }

    public virtual void OnEnable()
    {
        #if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChange;
        #endif
    }

    #if UNITY_EDITOR
    private void OnPlayModeStateChange(PlayModeStateChange change)
    {
        if (change == PlayModeStateChange.EnteredEditMode)
        {
            OnEnterPlayMode();
        }
        else if (change == PlayModeStateChange.ExitingPlayMode)
        {
            OnExitPlayMode();
        }
    }
    #endif


}