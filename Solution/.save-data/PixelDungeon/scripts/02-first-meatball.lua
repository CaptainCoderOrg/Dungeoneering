context.AddPartyMember(1)
context.WriteInfo("Kordanor joins the party!")
local setup = [[
     ####
     ####
   ##1##2##
   ########
   ##M##M##
     #MM#
     ####
]]
context.StartCombat(setup, "02-rescue-kordanor.lua", "02-lose-kordanor.lua")