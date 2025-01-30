context.ExitCombat()
context.SetPlayerView(8, 13, West)
context.HealParty()

local dialogue = Dialogue([[You awaken in the lunch room. You must have had a nightmare!]])
dialogue.AddOption(CloseDialogueOption("Wake up"))
dialogue.AddOption(RunScriptDialogueOption("Cheat", "ez-mode.lua"))
context.ShowDialogue(dialogue)

local success = context.GetVariable("final-0") and
  context.GetVariable("final-1") and
  context.GetVariable("final-2") and
  context.GetVariable("final-3")

if not success then
    context.SetVariable("respawn-meatballs", true)
    context.SetVariable("final-count", 0)
    context.SetVariable("final-0", false)
    context.SetVariable("final-1", false)
    context.SetVariable("final-2", false)
    context.SetVariable("final-3", false)
end