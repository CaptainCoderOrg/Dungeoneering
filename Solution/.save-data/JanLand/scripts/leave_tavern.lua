if context.GetVariable("in-tavern") then
    context.WriteInfo("The fresh air embraces you.")
    context.SetVariable("in-tavern", false)
end