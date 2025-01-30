using System.Collections;

using CaptainCoder.Dungeoneering.Player;
using CaptainCoder.Dungeoneering.Player.Unity;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace CaptainCoder.Dungeoneering.Unity;

public class GetInputController : MonoBehaviour
{
    public static GetInputController Shared { get; private set; } = default!;
    public GetInputController() { Shared = this; }
    private IButtonSetter _setter = default!;
    public TextMeshProUGUI KeyText = default!;

    public void OnEnable()
    {
        PlayerInputHandler.Shared.gameObject.SetActive(false);
    }

    public void OnDisable()
    {
        PlayerInputHandler.Shared.gameObject.SetActive(true);
    }


    public void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey && e.type == EventType.KeyDown && e.keyCode is not KeyCode.None)
        {
            Debug.Log($"Key pressed: {e.keyCode}");
            _setter.SetButton(e.keyCode);
            StartCoroutine(CloseNextFrame());
        }
    }

    IEnumerator CloseNextFrame()
    {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
    }

    public void Initialize(IButtonSetter button)
    {
        _setter = button;
        KeyText.text = $"Setting {button.ControlName} Key";
        gameObject.SetActive(true);
    }

}

public interface IButtonSetter
{
    public void SetButton(KeyCode code);
    public string ControlName { get; }
    public string Binding { get; }
    public void Render();
}

public class MovementControlButton : MonoBehaviour, IButtonSetter
{
    public TextMeshProUGUI ButtonText = default!;
    public Button Button = default!;
    public MovementAction Control;
    public string ControlName => Control.ToString();
    public string Binding => PlayerInputHandler.Shared.GetKeys(Control);
    public void OnEnable()
    {
        Render();
        Button.onClick.AddListener(OnClick);
    }

    public void OnDisable()
    {
        Button.onClick.RemoveListener(OnClick);
    }

    public void OnClick() => GetInputController.Shared.Initialize(this);

    public void SetButton(KeyCode code)
    {
        PlayerInputHandler.Shared.SetKey(Control, code);
        Render();
    }

    public void Render() => ButtonText.text = Binding;
}

public class MenuControlButton : MonoBehaviour, IButtonSetter

{
    public TextMeshProUGUI ButtonText = default!;
    public Button Button = default!;
    public MenuControl Control;

    public string ControlName => Control.ToString();
    public string Binding => PlayerInputHandler.Shared.GetKeys(Control);

    public void OnEnable()
    {
        Render();
        Button.onClick.AddListener(OnClick);
    }

    public void OnDisable()
    {
        Button.onClick.RemoveListener(OnClick);
    }

    public void OnClick() => GetInputController.Shared.Initialize(this);

    public void SetButton(KeyCode code)
    {
        PlayerInputHandler.Shared.SetKey(Control, code);
        Render();
    }
    public void Render() => ButtonText.text = Binding;
}