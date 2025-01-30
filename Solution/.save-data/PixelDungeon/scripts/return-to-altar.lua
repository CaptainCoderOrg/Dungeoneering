context.ExitCombat()
context.SetPlayerView(8, 13, West)
context.HealParty()

local dialogue = Dialogue([[You awaken in the lunch room. You must have had a nightmare!]])
dialogue.AddOption(CloseDialogueOption("Wake up"))
dialogue.AddOption(RunScriptDialogueOption("Cheat", "ez-mode.lua"))
context.ShowDialogue(dialogue)