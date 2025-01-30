if not context.GetVariable("green-room-desc") then
    context.WriteInfo("This appears to be the <color=red>employee</color> break room.")
    context.SetVariable("green-room-desc", true)
end