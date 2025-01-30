local success = context.GetVariable("final-0") and
  context.GetVariable("final-1") and
  context.GetVariable("final-2") and
  context.GetVariable("final-3")

local any = context.GetVariable("final-0") or
  context.GetVariable("final-1") or
  context.GetVariable("final-2") or
  context.GetVariable("final-3")

if any and not success then
  local dialogue = Dialogue("[If you leave, the meatballs in this room will have time to regenerate. Are you sure you want to leave?")
  dialogue.AddOption(RunScriptDialogueOption("Stay", "000-stay.lua"))
  dialogue.AddOption(RunScriptDialogueOption("Leave", "000-leave.lua"))
  context.ShowDialogue(dialogue)
end