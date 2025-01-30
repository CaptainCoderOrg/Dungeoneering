if context.GetVariable("has-blue-key") then
    local dialogue = Dialogue([["Before you is a swirling portal. It leads back home... you know it in your heart."]])
    local enter = Dialogue([[As you move to enter, the smell of rotting meat stops you. You can't move... you're stuck in place.]])
    local eyekeyuh = Dialogue([[Eye Key Uh in a weakend state descends upon you!]])
    eyekeyuh.AddOption(RunScriptDialogueOption("Fight!", "zzzz-fight-eyekeyuh.lua"))
    enter.AddOption(ContinueDialogueOption("Uh oh...", eyekeyuh))
    dialogue.AddOption(ContinueDialogueOption("Enter Portal", enter))
    dialogue.AddOption(RunScriptDialogueOption("Leave", "push-back.lua"))
    context.ShowDialogue(dialogue)
end