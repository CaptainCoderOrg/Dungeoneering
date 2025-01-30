local chestDialogue = Dialogue(
[[There is a poorly constructed dresser here.]]
)


chestDialogue.AddOption(RunScriptDialogueOption("Open Dresser", "00open-dresser.lua"));
chestDialogue.AddOption(CloseDialogueOption("Leave"))

context.ShowDialogue(chestDialogue)