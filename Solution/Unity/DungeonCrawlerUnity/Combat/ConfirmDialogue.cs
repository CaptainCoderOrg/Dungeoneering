using CaptainCoder.Dungeoneering.Player.Unity;

using TMPro;

using UnityEngine;

namespace CaptainCoder.DungeonCrawler.Combat.Unity;
public class ConfirmDialogue : MonoBehaviour
{
    public static ConfirmDialogue Shared { get; private set; } = default!;
    public PlayerInputMapping PlayerInputMapping = default!;
    public TextMeshProUGUI Message = default!;
    public MenuItem ConfirmItem = default!;
    public MenuItem CancelItem = default!;
    private bool _isConfirmSelected;
    private Action _onConfirm = default!;
    private Action _onCancel = default!;

    public ConfirmDialogue()
    {
        Shared = this;
    }

    public void Update()
    {
        foreach (var mapping in PlayerInputMapping.MenuActionMappings)
        {
            if (Input.GetKeyDown(mapping.Key))
            {
                HandleInput(mapping.Action);
            }
        }
    }

    private void HandleInput(MenuControl action)
    {
        Action method = action switch
        {
            MenuControl.Up or MenuControl.Down or MenuControl.Left or MenuControl.Right => Toggle,
            MenuControl.Select => Confirm,
            MenuControl.Cancel => Cancel,
            _ => throw new NotImplementedException($"Unknown action: {action}"),
        };
        method.Invoke();
    }

    private void Toggle()
    {
        _isConfirmSelected = !_isConfirmSelected;
        ConfirmItem.IsSelected = _isConfirmSelected;
        CancelItem.IsSelected = !_isConfirmSelected;
    }

    private void Confirm()
    {
        if (_isConfirmSelected)
        {
            gameObject.SetActive(false);
            _onConfirm.Invoke();
        }
        else
        {
            Cancel();
        }
    }

    private void Cancel()
    {
        gameObject.SetActive(false);
        _onCancel.Invoke();
    }

    public void Initialize(string message, Action onConfirm, Action onCancel)
    {
        Message.text = message;
        _onConfirm = onConfirm;
        _onCancel = onCancel;
        _isConfirmSelected = true;
        ConfirmItem.IsSelected = true;
        CancelItem.IsSelected = false;
        gameObject.SetActive(true);
    }
}