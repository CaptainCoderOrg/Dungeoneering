if not context.GetVariable("has-key") then
    context.WriteInfo("The door is locked")
    context.SetPlayerPosition(10, 11);
elseif not context.GetVariable("first-exit") then
    context.WriteInfo("Zooperdan uses the key to unlock the door.")
    context.WriteInfo([["What a strange hallway..."]])
    context.SetVariable("first-exit", true)
end

