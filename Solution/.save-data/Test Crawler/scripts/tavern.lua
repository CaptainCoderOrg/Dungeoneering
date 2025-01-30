if not context.GetVariable("in-tavern") then
    context.SetVariable("in-tavern", true)
    context.WriteInfo("The smell of ale surrounds you.")
end