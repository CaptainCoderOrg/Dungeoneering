local chance = math.random(1, 4)
if chance == 1 then
    local dialogue = Dialogue([[You accidentally bump shoulders with a disgruntled employee! 'HEY! Watch where yer goin!' The employee raises their fists ready to fight.]])
    dialogue.AddOption(RunScriptDialogueOption("Apologize", "03-apologize-employee.lua"))
    dialogue.AddOption(RunScriptDialogueOption("Fight Employee", "03-fight-employee.lua"))
    context.ShowDialogue(dialogue)
end