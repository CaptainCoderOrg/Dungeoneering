context.HealParty()
local chance = math.random(1, 4)
local map = ""

map = [[
   3###
   2###
   1##P
   4###
]]


context.StartCombat(map, "explain-combat-3.lua", "explain-combat-2.lua")