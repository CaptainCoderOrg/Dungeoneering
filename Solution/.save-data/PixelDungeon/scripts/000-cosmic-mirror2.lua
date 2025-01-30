if not context.GetVariable("second-cosmic-mirror") then
    if context.GetVariable("ronadrok-in-party") then
        local dialogue = Dialogue(
        [[There is a cosmic mirror here. The colors are beyond comprehension. Who will look at their reflection?]])
        dialogue.AddOption(RunScriptDialogueOption("Zooperdan", "000-zooperdan-looks.lua"))
        dialogue.AddOption(CloseDialogueOption("Leave"))
        context.ShowDialogue(dialogue)
    else
        local dialogue = Dialogue(
        [[There is a cosmic mirror here. The colors are beyond comprehension. Who will look at their reflection?]])
        dialogue.AddOption(RunScriptDialogueOption("Kordanor", "000-kordanor-looks.lua"))
        dialogue.AddOption(CloseDialogueOption("Leave"))
        context.ShowDialogue(dialogue)
    end
else
    local dialogue = Dialogue([[The mirror has lost all of its color]])
    dialogue.AddOption(CloseDialogueOption("Leave"))
    context.ShowDialogue(dialogue)
end
