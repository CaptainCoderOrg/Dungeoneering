context.WriteInfo("It's a meat locker! And it is filled with Cosmic Meatballs!")
local setup = [[
    M####    ###D 
    #M###   3#####
    #M#######1####B
    M########2####B
            4#####
             ###H
]]
context.StartCombat(setup, "03-win-meat-locker.lua", "return-to-altar.lua")