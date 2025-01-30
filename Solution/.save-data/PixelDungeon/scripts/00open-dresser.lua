if not context.GetVariable("has-key") then
    context.WriteInfo("Zooperdan finds a key.")
    context.SetVariable("has-key", true)
else
    context.WriteInfo("The dresser is empty.")
end

