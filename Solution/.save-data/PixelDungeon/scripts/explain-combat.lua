context.HealParty()
local dialogue = Dialogue([[Each turn consists of two phases: 

1. Spend 2 Action Points to acquire movement points and attack points.

2. Spend movement and action points to fight enemies]])
local more = Dialogue([[In the following combat, spend your action points to gain 2 attack points. Then spend your attack points to defeat the skeleton.]])
more.AddOption(RunScriptDialogueOption("Fight a Skeleton", "fight-a-skeleton.lua"))
more.AddOption(CloseDialogueOption("Leave"))
dialogue.AddOption(ContinueDialogueOption("More", more))
context.ShowDialogue(dialogue)