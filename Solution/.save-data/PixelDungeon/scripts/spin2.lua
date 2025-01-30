function NewFacing()
    local dir = math.random(0, 3)
    if (dir == context.PlayerView.Facing) then
        return NewFacing()
    end
    return dir
end
context.SetPlayerFacing(NewFacing())