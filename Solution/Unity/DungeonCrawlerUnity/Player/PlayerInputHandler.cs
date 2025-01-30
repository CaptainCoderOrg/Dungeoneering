using UnityEngine;
using UnityEngine.Events;
namespace CaptainCoder.Dungeoneering.Player.Unity;

public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Shared { get; private set; } = default!;
    public PlayerInputHandler() { Shared = this; }
    [field: SerializeField]
    public PlayerInputMapping InputMapping { get; set; } = default!;
    public UnityEvent<MovementAction>? OnMovementAction;
    public UnityEvent<MenuControl>? OnMenuControl;
    public void Update()
    {
        if (_crawlingEnabled)
        {
            foreach (MovementActionMapping mapping in InputMapping.MovementActions)
            {
                if (Input.GetKeyDown(mapping.Key))
                {
                    OnMovementAction?.Invoke(mapping.Action);
                }
            }
        }

        foreach (MenuActionMapping mapping in InputMapping.MenuActionMappings)
        {
            if (Input.GetKeyDown(mapping.Key))
            {
                OnMenuControl?.Invoke(mapping.Action);
            }
        }
    }
    internal string GetKeys(MenuControl control) => string.Join(" or ", InputMapping.MenuActionMappings.Where(mapping => mapping.Action == control).Select(m => m.Key.ToString()));
    internal string GetKeys(MovementAction control) => string.Join(" or ", InputMapping.MovementActions.Where(mapping => mapping.Action == control).Select(m => m.Key.ToString()));

    internal void SetKey(MovementAction control, KeyCode code)
    {
        for (int ix = 0; ix < InputMapping.MovementActions.Length; ix++)
        {
            MovementActionMapping mapping = InputMapping.MovementActions[ix];
            if (mapping.Action == control)
            {
                InputMapping.MovementActions[ix].Key = code;
            }
        }
    }

    internal void SetKey(MenuControl control, KeyCode code)
    {
        for (int ix = 0; ix < InputMapping.MovementActions.Length; ix++)
        {
            MenuActionMapping mapping = InputMapping.MenuActionMappings[ix];
            if (mapping.Action == control)
            {
                InputMapping.MenuActionMappings[ix].Key = code;
            }
        }
    }

    private bool _crawlingEnabled = true;

    internal void DisableCrawling() => _crawlingEnabled = false;

    internal void EnableCrawling() => _crawlingEnabled = true;
}


[CreateAssetMenu(fileName = "PlayerInputMapping", menuName = "Config/PlayerInputMapping")]
public class PlayerInputMapping : ScriptableObject
{
    [field: SerializeField]
    public MovementActionMapping[] MovementActions { get; set; } = default!;
    [field: SerializeField]
    public MenuActionMapping[] MenuActionMappings { get; set; } = default!;

    public void OnMenuAction(Action<MenuControl> handler)
    {
        foreach (var mapping in MenuActionMappings)
        {
            if (Input.GetKeyDown(mapping.Key))
            {
                handler.Invoke(mapping.Action);
            }
        }
    }
}

[Serializable]
public class MovementActionMapping
{
    public KeyCode Key;
    public MovementAction Action;
}

[Serializable]
public class MenuActionMapping : ActionMapping<MenuControl> { }

public enum MenuControl
{
    Up,
    Down,
    Left,
    Right,
    Select,
    Cancel
}

public class ActionMapping<T>
{
    public KeyCode Key;
    public T? Action;
}