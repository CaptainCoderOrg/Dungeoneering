local missedDoor = 1
if not context.GetVariable("missed-door") then
    context.SetVariable("missed-door", 1)
else
    missedDoor = context.GetVariable("missed-door") + 1
    context.SetVariable("missed-door", missedDoor)
end

context.WriteInfo("A gust of <color=red>rank</color> air blows at you from the <b><color=green>west</color></b>.")

if missedDoor >= 4 and not context.GetVariable("defeated-green-room.lua") then
    context.WriteInfo("There is something odd about this wall...")
    context.SetWallTextureAt(13, 21, West, "green-secret-door.png")
end

