context.HealParty()
local chance = math.random(1, 4)
local map = ""

map = [[
   3###
   2###
   1S##
   4###
]]


context.StartCombat(map, "explain-combat-2.lua", "explain-combat.lua")