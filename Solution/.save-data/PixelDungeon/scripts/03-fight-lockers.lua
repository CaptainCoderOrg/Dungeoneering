local layout = math.random(1, 4)
local map = ''
if not context.GetVariable("has-purple-key") or layout == 1 then
    map = [[
        #W##
        1##4
        #23#
        ##W#
    ]]
elseif layout == 2 then
    map = [[
           W
          #4#
         #####
        #######
       W3#####1W
        #######
         #####
          #2#
           W
         ]]
elseif layout == 3 then
    map = [[
           W
          1#3
         #####
        #######
         #####
          2#4
           W
         ]]
else
    map = [[
           1
           W
          ### 
        3#####4
         #####
           W
           2
         ]]
end
context.StartCombat(map, "03-defeat-locker.lua", "return-to-altar.lua")