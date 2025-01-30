if not context.GetVariable("has-purple-key") then
    context.WriteInfo("The door is locked")
    context.SetPlayerPosition(8, 6);
elseif not context.GetVariable("first-purple-exit") then
    context.WriteInfo("The purple key opens the door")
    context.SetVariable("first-purple-exit", true)
end

