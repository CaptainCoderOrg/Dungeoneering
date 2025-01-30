local dialogue = Dialogue([["You've had your fill of swedish meats and you want to go home..."]])
local enter = Dialogue([[You gather your courage and enter the portal unsure of what will come. But it must be better than this.]])
enter.AddOption(RunScriptDialogueOption("Continue", "zzzz-show-credits.lua"))
dialogue.AddOption(ContinueDialogueOption("Enter Portal", enter))
context.ShowDialogue(dialogue)
    