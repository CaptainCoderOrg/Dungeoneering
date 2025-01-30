if not context.GetVariable("green-room-desc2") then
    context.WriteInfo("Disgruntled <color=red>employees</color> wander about muttering <b>profanties</b>. You better be careful!")
    context.SetVariable("green-room-desc2", true)
end