function NewFacing()
    return (context.PlayerView.Facing + 2) % 4
end
context.WriteInfo("A disgrunteled <color=red>employee</color> crashes into you and walks off! How rude!")
context.SetPlayerFacing(NewFacing())