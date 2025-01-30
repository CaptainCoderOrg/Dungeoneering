local chance = math.random(1, 4)
local apologized = 1
if context.GetVariable("apologized") == nil then
    context.SetVariable("apologized", 1)
else
    apologized = context.GetVariable("apologized") + 1
    context.SetVariable("apologized", apologized)
end
if context.GetVariable("has-employee-outfit") then
    context.WriteInfo("'Whatever...', the <color=red>employee</color> mutters and walks away.")
elseif apologized == 1 then
    context.WriteInfo("'Whatever...', the <color=red>employee</color> mutters and walks away.")
elseif apologized == 2 then
    local dialogue = Dialogue([['Sorry is for suckers!', the employee attacks you!]])
    dialogue.AddOption(RunScriptDialogueOption("Fight Employee", "03-fight-employee.lua"))
    context.ShowDialogue(dialogue)
elseif chance == 1 then
    local dialogue = Dialogue([['Sorry is for suckers!', the employee attacks you!]])
    dialogue.AddOption(RunScriptDialogueOption("Fight Employee", "03-fight-employee.lua"))
    context.ShowDialogue(dialogue)
else
    context.WriteInfo("'Whatever...', the <color=red>employee</color> mutters and walks away.")
end


