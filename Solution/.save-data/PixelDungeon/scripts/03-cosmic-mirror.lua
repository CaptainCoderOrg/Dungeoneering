if not context.GetVariable("first-cosmic-mirror") then
    local dialogue = Dialogue([[There is a cosmic mirror here. The colors are beyond comprehension. Who will look at their reflection?]])
    dialogue.AddOption(RunScriptDialogueOption("Zooperdan", "03-zooperdan-looks.lua"))
    dialogue.AddOption(RunScriptDialogueOption("Kordanor", "03-kordanor-looks.lua"))
    dialogue.AddOption(CloseDialogueOption("Leave"))
    context.ShowDialogue(dialogue)
else
    local dialogue = Dialogue([[The mirror has lost all of its color]])
    dialogue.AddOption(CloseDialogueOption("Leave"))
    context.ShowDialogue(dialogue)
end
