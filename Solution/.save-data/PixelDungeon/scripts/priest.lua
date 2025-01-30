local priestDialogue = Dialogue(
[[<size=30><b>Morlin</b></size>
Hello and welcome to <color=green>Alkabar</color>! I am <b>Morlin</b>, the local priest. 

I can offer you healing services!
]]
)

local heal = Dialogue("Healing services are 50 gold")
heal.AddOption(RunScriptDialogueOption("Pay 50 Gold", "heal.lua"))
heal.AddOption(ContinueDialogueOption("No thanks", priestDialogue))

priestDialogue.AddOption(ContinueDialogueOption("Heal", heal));
priestDialogue.AddOption(RunScriptDialogueOption("Make a Donation", "donation.lua"))
priestDialogue.AddOption(CloseDialogueOption("Leave"))

context.ShowDialogue(priestDialogue)