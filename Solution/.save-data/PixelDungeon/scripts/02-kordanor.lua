if not context.GetVariable("kordanor-saved") then
    local dialogue = Dialogue([[As you approach, you realize the cosmic meatball is draining the mans energy. It is going to kill him!]])
    dialogue.AddOption(RunScriptDialogueOption("Save Man", "02-first-meatball.lua"))
    dialogue.AddOption(CloseDialogueOption("Leave"))
    context.ShowDialogue(dialogue)
end