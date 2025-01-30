context.HealParty()
local dialogue = Dialogue([[Hopefully, you now know the basics of combat. As you play, you will discover more tactics to use. For example, you can spend energy to increase your weapons damage. Or, use the rest action to restore your energy. As your party grows, the combinations become more powerful.]])
dialogue.AddOption(CloseDialogueOption("Back to the Game"))
context.ShowDialogue(dialogue)