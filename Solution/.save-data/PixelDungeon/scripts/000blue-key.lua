if not context.GetVariable("has-blue-key") then
    context.WriteInfo("The door is locked")
    context.PushBack()
elseif not context.GetVariable("first-blue-exit") then
    context.WriteInfo("The blue key opens the door")
    context.SetVariable("first-blue-exit", true)
end

