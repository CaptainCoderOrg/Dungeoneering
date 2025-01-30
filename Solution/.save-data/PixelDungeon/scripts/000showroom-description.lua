if not context.GetVariable("final-count") then
    context.SetVariable("final-count", 0)
end

if not context.GetVariable("entered-show-room") then
    context.SetVariable("entered-show-room", true)
    context.WriteInfo("This appears to be the cosmic show room. It is filled with broken furniture, meatballs, and angry customers.")
end

if context.GetVariable("respawn-meatballs") then
    local dialogue = Dialogue([[While you were away, it appears the bloodshot meatballs have regrown! You'll have to start over. You'll have to defeat all of them without leaving the showroom.]])
    dialogue.AddOption(CloseDialogueOption("Let's go!"))
    context.SetVariable("respawn-meatballs", false)
end