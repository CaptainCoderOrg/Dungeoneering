local chance = math.random(1, 4)
local map = ""
if chance == 1 then
    map = [[
        ##      ##
        ##      ##
        ##      12
        #B######34
        ######H###
    ]]
elseif chance == 2 then
    map = [[
        #1#B####H#
        ######2###
        ##D#######
        4####H###3
    ]]
elseif chance == 3 then
    map = [[
        #H      ##
        ##      #B
        #1      ##
        #######2##
        ##########
        ##########
        4D###3####
    ]]
else
    map = [[
        #####H####
        ##2#######
        #1      ##
        ##      ##
        #B      4#
        ##D####3##
        ##########
    ]]
end

context.StartCombat(map, "03-defeat-employees.lua", "return-to-altar.lua")