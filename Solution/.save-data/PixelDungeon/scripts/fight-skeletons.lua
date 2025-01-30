local chance = math.random(1, 4)
local map = ""
if chance == 1 then
    map = [[
    4###
   1S####
   2####S
    3###
]]
elseif chance == 2 then
    map = [[
        #1S######S
        #S####2###
        ##########
        4########3

    ]]
elseif chance == 3 then
    map = [[
        31S####S#S
        42########

    ]]
else
    map = [[
         34
        ####
       1S###S
       2#####
        ####
         ##
    ]]
end


context.StartCombat(map, "defeat-skeletons.lua", "return-to-altar.lua")