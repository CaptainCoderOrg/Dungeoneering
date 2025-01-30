local success = context.GetVariable("final-0") and
  context.GetVariable("final-1") and
  context.GetVariable("final-2") and
  context.GetVariable("final-3")
if success then
    local dialogue = Dialogue("The meat has stopped moving. It appears Eye Key Uh has lost control of them.")
    dialogue.AddOption(RunScriptDialogueOption("Search Remains", "000-loot-remains.lua"))
    dialogue.AddOption(CloseDialogueOption("Leave"))
    context.ShowDialogue(dialogue)
elseif not context.GetVariable("final-1") then
    local dialogue = Dialogue([[Four bloodshot meatballs stand guard over cosmic furniture out of control.]])
    dialogue.AddOption(RunScriptDialogueOption("Fight!", "000-bloodshot1.lua"))
    dialogue.AddOption(CloseDialogueOption("Leave"))
    context.ShowDialogue(dialogue)
else
    context.WriteInfo("The bloodshot meatballs appears to be reforming... you better hurry.")
end