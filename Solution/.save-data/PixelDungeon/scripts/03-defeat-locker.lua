if not context.GetVariable("has-purple-key") then
    local dialogue = Dialogue([[While gently searching the wardrobes, you find a purple key!]])
    dialogue.AddOption(RunScriptDialogueOption("What More?", "03-locker-loot.lua"))
    context.SetVariable("has-purple-key", true)
    context.ShowDialogue(dialogue)
else
    local dialogue = Dialogue([[While gently searching the wardrobes, you find a weapon!]])
    dialogue.AddOption(RunScriptDialogueOption("Gimme!", "03-locker-loot.lua"))
    context.ShowDialogue(dialogue)
end
