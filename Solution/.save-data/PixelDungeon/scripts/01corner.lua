local count = 0
if not context.GetVariable("corner-count") then
    count = 1
else
    count = context.GetVariable("corner-count")
end
context.SetVariable("corner-count", count + 1)

if count == 1 then
    context.WriteInfo([[Zooperdan hears a whisper, "Come to me."]])
elseif count == 2 then
    context.WriteInfo([[The smell of onions fills the air.]])
elseif count == 3 then
    context.WriteInfo([["Don't stop.", the voice whispers]])
elseif count == 4 then
    context.WriteInfo([[The air grows thick and dank.]])
elseif count == 5 then
    context.WriteInfo([["You're almost here..."]])
elseif count == 6 then
    context.WriteInfo([[The smell of rotting meat is almost too much to bear]])
else
    context.RunScript("01ikea.lua")
end

