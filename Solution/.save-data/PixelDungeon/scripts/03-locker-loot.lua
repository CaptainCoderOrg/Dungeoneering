-- Define an array of small items found in a furniture store
local items = {
    "Backpack",
    "Lunchbox",
    "Headphones",
    "Notebook",
    "Pen",
    "Bottle",
    "Button",
    "Charger",
    "Pencil",
    "Thingy",
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
    "ethereal",
    "dimensional",
    "celestial",
    "arcane",
    "nebulous",
    "eldritch",
    "phantasmal",
    "astral",
    "voidal",
    "mystic"
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
local min = math.random(3, 5)
local range = math.random(3, 5)
context.GiveWeapon(name, min, min + range)