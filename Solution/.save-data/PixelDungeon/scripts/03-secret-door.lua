if not context.GetVariable("defeated-green-room.lua") then
    local dialogue = Dialogue([[There is something odd about this wall...]])
    local inspect = Dialogue([[On closer inspection, you discover this is a locker!]])
    local meat = Dialogue([[It's a meat locker! And it is full of cosmic meatballs! Their gaze turns toward you. It seems you've also caught the attention of the employees behind you...]])
    meat.AddOption(RunScriptDialogueOption("Fight!", "03-open-locker.lua"))
    inspect.AddOption(ContinueDialogueOption("Open Locker", meat))
    inspect.AddOption(CloseDialogueOption("Leave"))

    dialogue.AddOption(ContinueDialogueOption("Inspect Wall", inspect))
    dialogue.AddOption(CloseDialogueOption("Leave"))

    context.PushBack()
    context.ShowDialogue(dialogue)
elseif not context.GetVariable("enter-meat-locker") then
    context.WriteInfo("You enter the meat locker. There is a strange mirror in the corner.")
    context.SetVariable("enter-meat-locker", true)
end