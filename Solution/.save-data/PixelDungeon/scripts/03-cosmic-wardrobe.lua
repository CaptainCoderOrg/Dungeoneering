if context.GetVariable("has-employee-outfit") then
    context.WriteInfo("Colorful splinters are scattered throughout this area left over from the wardrobe you destroyed.")
else
    local dialogue = Dialogue([[There is cosmic wardrobe here. It probably contains the employee uniforms.]])
    if context.GetVariable("ronadrok-in-party") then
        local opened = Dialogue([[Ronadrok reaches for the wardrobe. Suddenly, it springs to life! Several disgruntled employees approach from behind.]])
        opened.AddOption(RunScriptDialogueOption("Fight!", "03-fight-wardrobe.lua"))
        dialogue.AddOption(ContinueDialogueOption("Open Wardrobe", opened))
    elseif context.GetVariable("nadrepooz-in-party") then
        local opened = Dialogue([[Nadrepooz reaches for the wardrobe. Suddenly, it springs to life! Several disgruntled employees approach from behind.]])
        opened.AddOption(RunScriptDialogueOption("Fight!", "03-fight-wardrobe.lua"))
        dialogue.AddOption(ContinueDialogueOption("Open Wardrobe", opened))
    else
        local cannotopen = Dialogue([[Your hands pass through the wardrobe. Apparently only cosmic beings can interact with this object.]])
        cannotopen.AddOption(CloseDialogueOption("Leave"))
        dialogue.AddOption(ContinueDialogueOption("Open Wardrobe", cannotopen))
    end

    dialogue.AddOption(CloseDialogueOption("Leave"))
    context.ShowDialogue(dialogue)
end