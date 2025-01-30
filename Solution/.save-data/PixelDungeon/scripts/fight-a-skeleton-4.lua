context.HealParty()
local chance = math.random(1, 4)
local map = ""

map = [[
   3##
   2######
   1#####P
   4##
]]


context.StartCombat(map, "explain-combat-5.lua", "explain-combat-4.lua")