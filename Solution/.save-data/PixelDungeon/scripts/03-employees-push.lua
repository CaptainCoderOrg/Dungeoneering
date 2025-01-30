if not context.GetVariable("has-employee-outfit") then
    context.ShowDialogue(Dialogue([[A disgruntled employee stops you. 'Hey! Can't you read!? The sign says employees only.']]))
    context.PushBack()
end