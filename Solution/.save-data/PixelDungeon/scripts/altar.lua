local dialogue = Dialogue([[Before you is a buffet table filled with köttbullar.]])
dialogue.AddOption(RunScriptDialogueOption("Eat You Fill", "eat.lua"))
context.ShowDialogue(dialogue)