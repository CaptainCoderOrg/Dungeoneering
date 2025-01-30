-- Define an array of tools commonly found
local tools = {
    "Hammer",
    "Wrench",
    "Drill",
    "Saw",
    "Pliers",
    "Screwdriver",
    "Tape Measure",  -- Added tool
    "Level",         -- Added tool
    "Paint Brush", -- Added tool
    "Chisel"         -- Added tool
}

-- Function to randomly select a tool
function selectRandomTool(tools)
    -- #tools gives the size of the table
    local index = math.random(1, #tools)
    return tools[index]
end

-- Select a random tool
local randomTool = selectRandomTool(tools)

local smells = {
    "minty",
    "floral",
    "citrus",
    "woody",
    "earthy",
    "spicy",
    "smoky",
    "fresh",
    "sour",
    "sweet",
}

-- Function to randomly select a smell
function selectRandomSmell(smells)
    -- #smells gives the size of the table
    local index = math.random(1, #smells)
    return smells[index]
end

-- Select a random smell
local randomSmell = selectRandomSmell(smells)
local name = randomSmell .. " " .. randomTool
local min = math.random(2, 4)
local range = math.random(2, 3)
context.WriteInfo("The employees drops their " .. name .. " and run away!")
context.GiveWeapon(name, min, min + range)