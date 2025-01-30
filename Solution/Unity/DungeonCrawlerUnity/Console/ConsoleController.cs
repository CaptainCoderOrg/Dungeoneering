using System.Collections;

using CaptainCoder.Dungeoneering.Lua;
using CaptainCoder.Dungeoneering.Player.Unity;
using CaptainCoder.Dungeoneering.Unity;

using TMPro;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Scripting.Unity;

public class ConsoleController : MonoBehaviour
{
    [field: SerializeField]
    public TextMeshProUGUI TextTemplate { get; private set; } = default!;
    [field: SerializeField]
    public TMP_InputField UserInputField { get; private set; } = default!;
    [field: SerializeField]
    public Transform ConsoleOutput { get; private set; } = default!;
    [field: SerializeField]
    public CrawlingModeController GameController { get; private set; } = default!;
    [field: SerializeField]
    public PlayerInputHandler PlayerMovementController { get; private set; } = default!;
    [field: SerializeField]
    public KeyCode DeactivateKey { get; private set; } = KeyCode.Escape;

    private readonly List<string> _commandHistory = new();
    private int _historyIx = 0;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            string command = UserInputField.text;
            HandleCommand(command);
            UserInputField.SetTextWithoutNotify(string.Empty);
            StartCoroutine(FocusInputField());
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            NavigateCommandHistory(-1);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            NavigateCommandHistory(1);
        }


        if (Input.GetKeyDown(DeactivateKey))
        {
            gameObject.SetActive(false);
        }
    }

    private void NavigateCommandHistory(int offset)
    {
        _historyIx = Math.Clamp(_historyIx + offset, 0, _commandHistory.Count);
        if (_historyIx == _commandHistory.Count)
        {
            UserInputField.SetTextWithoutNotify(string.Empty);
        }
        else
        {
            UserInputField.SetTextWithoutNotify(_commandHistory[_historyIx]);
        }
        StartCoroutine(FocusInputField());
    }

    public void OnEnable()
    {
        StartCoroutine(FocusInputField());
        PlayerMovementController.gameObject.SetActive(false);
    }

    public void OnDisable()
    {
        PlayerMovementController.gameObject.SetActive(true);
    }

    public void HandleCommand(string command)
    {
        command = command.Trim();
        _commandHistory.Add(command);
        _historyIx = _commandHistory.Count;
        if (command == string.Empty) { return; }
        Write($" > {command}");
        try
        {
            var result = Interpreter.EvalLua<object>(command, GameController);
            if (result is not null)
            {
                Write(result.ToString());
            }
        }
        catch (Exception e)
        {
            Write($"Exception occurred: {e.Message}");
            throw e;
        }
    }

    private void Write(string text)
    {
        Debug.Log(text);
        TextMeshProUGUI newText = Instantiate(TextTemplate, ConsoleOutput);
        newText.text = text;
    }

    private IEnumerator FocusInputField()
    {
        yield return new WaitForEndOfFrame();
        UserInputField.ActivateInputField();
    }
}

public class ConsoleToggler : MonoBehaviour
{
    [field: SerializeField]
    public ConsoleController ConsoleController { get; private set; } = default!;
    [field: SerializeField]
    public KeyCode ActivateKey { get; private set; } = KeyCode.BackQuote;

    public void Update()
    {
        if (Input.GetKeyDown(ActivateKey))
        {
            ConsoleController.gameObject.SetActive(true);
        }
    }
}