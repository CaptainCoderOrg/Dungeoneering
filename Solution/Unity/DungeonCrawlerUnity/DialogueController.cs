using CaptainCoder.Dungeoneering.Lua;
using CaptainCoder.Dungeoneering.Lua.Dialogue;
using CaptainCoder.Dungeoneering.Player.Unity;
using CaptainCoder.Unity;

using TMPro;

using UnityEngine;

namespace CaptainCoder.Dungeoneering.Unity;

public class DialogueController : MonoBehaviour
{
    [field: SerializeField]
    public TextMeshProUGUI Text { get; set; } = default!;

    [field: SerializeField]
    public Transform OptionsContainer { get; set; } = default!;
    [field: SerializeField]
    public DialogueOptionController OptionPrefab { get; set; } = default!;
    [field: SerializeField]
    public CrawlingModeController CrawlingController { get; set; } = default!;
    [field: SerializeField]
    public PlayerInputHandler PlayerInputHandler { get; set; } = default!;
    [field: SerializeField]
    public Color SelectedColor { get; set; } = Color.yellow;
    [field: SerializeField]
    public Color NormalColor { get; set; } = Color.black;
    private readonly List<DialogueOptionController> _allOptions = [];
    private int _selectedIx = 0;

    public void OnEnable()
    {
        PlayerInputHandler.Shared.DisableCrawling();
    }

    public void OnDisable()
    {
        PlayerInputHandler.Shared.EnableCrawling();
    }

    public void Show(Dialogue dialogue)
    {
        Text.text = dialogue.Text;

        List<DialogueOption> options = [.. dialogue.Options];
        if (options.Count == 0)
        {
            options.Add(new CloseDialogueOption("Close"));
        }

        _allOptions.Clear();
        _selectedIx = 0;
        OptionsContainer.DestroyAllChildren(Destroy);
        foreach (DialogueOption option in options)
        {
            DialogueOptionController optionController = Instantiate(OptionPrefab, OptionsContainer);
            optionController.Text.text = option.Label;
            optionController.Option = option;
            _allOptions.Add(optionController);
        }

        Next(0); // Selects the first option
        gameObject.SetActive(true);
    }

    private void Next(int delta)
    {
        DialogueOptionController prev = _allOptions[_selectedIx];
        _selectedIx = (_selectedIx + delta) switch
        {
            int ix when ix >= _allOptions.Count => 0,
            int ix when ix < 0 => _allOptions.Count - 1,
            int ix => ix,
        };
        DialogueOptionController current = _allOptions[_selectedIx];
        prev.Text.color = NormalColor;
        current.Text.color = SelectedColor;
    }

    public void Update() => PlayerInputHandler.Shared.InputMapping.OnMenuAction(HandleUserInput);

    public void HandleUserInput(MenuControl action)
    {
        Action result = action switch
        {
            MenuControl.Right or MenuControl.Down => () => Next(1),
            MenuControl.Left or MenuControl.Up => () => Next(-1),
            MenuControl.Select => ExecuteOption,
            // MenuControl.Exit => Close,
            _ => () => { }
        };
        result.Invoke();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void ExecuteOption()
    {
        DialogueOptionController controller = _allOptions[_selectedIx];
        Action action = controller.Option switch
        {
            CloseDialogueOption close => Close,
            ContinueDialogueOption option => () => Show(option.NextDialogue),
            RunScriptDialogueOption option => () => RunScriptAndClose(option.ScriptName),
            var unknown => throw new NotImplementedException($"Unknown option of type {unknown?.GetType()}: {unknown}."),
        };
        action.Invoke();
    }

    private void RunScriptAndClose(string scriptName)
    {
        Close();
        string script = CrawlingController.Manifest.Scripts[scriptName].Script;
        Interpreter.ExecLua(script, CrawlingController);
    }
}

[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogueOptionController : MonoBehaviour
{
    [field: SerializeField]
    public TextMeshProUGUI Text { get; set; } = default!;
    public DialogueOption Option { get; set; } = default!;

    public void Awake()
    {
        Text = GetComponent<TextMeshProUGUI>();
    }
}