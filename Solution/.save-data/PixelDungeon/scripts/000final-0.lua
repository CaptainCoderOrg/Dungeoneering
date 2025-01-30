local success = context.GetVariable("final-0") and
  context.GetVariable("final-1") and
  context.GetVariable("final-2") and
  context.GetVariable("final-3")
if success then
    local dialogue = Dialogue("The meat has stopped moving. It appears Eye Key Uh has lost control of them.")
    dialogue.AddOption(RunScriptDialogueOption("Search Remains", "000-loot-remains.lua"))
    dialogue.AddOption(CloseDialogueOption("Leave"))
    context.ShowDialogue(dialogue)
elseif not context.GetVariable("final-0") then
    local dialogue = Dialogue([[A bloodshot meatball gazes at a group of angry customers. The gaze is keeping them from attacking.]])
    dialogue.AddOption(RunScriptDialogueOption("Fight!", "000-bloodshot0.lua"))
    dialogue.AddOption(CloseDialogueOption("Leave"))
    context.ShowDialogue(dialogue)
else
    context.WriteInfo("The bloodshot meatball appears to be reforming... you better hurry.")
end