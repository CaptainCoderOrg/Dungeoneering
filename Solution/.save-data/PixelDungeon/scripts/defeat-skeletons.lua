-- Define an array of small items found in a furniture store
local items = {
    "Candle",
    "Vase",
    "Frame",
    "Lamp",
    "Rug",
    "Pillow",
}

-- Function to randomly select an item
function selectRandomItem(items)
    -- #items gives the size of the table
    local index = math.random(1, #items)
    return items[index]
end

-- Select a random item and print it
local randomItem = selectRandomItem(items)

local adjectives = {
    "squishy",
    "rubbery",
    "giggly",
    "wobbly",
    "squeaky",
    "bouncy",
    "fluffy",
    "sparkly",
    "slippery",
    "ticklish"
}

-- Function to randomly select an adjective
function selectRandomAdjective(adjectives)
    -- #adjectives gives the size of the table
    local index = math.random(1, #adjectives)
    return adjectives[index]
end

-- Select a random adjective and print it
local randomAdjective = selectRandomAdjective(adjectives)
local name = randomAdjective .. " " .. randomItem
local min = math.random(2, 3)
local range = math.random(1, 2)
context.WriteInfo("Searching the closet you find a " .. name)
context.GiveWeapon(name, min, min + range)