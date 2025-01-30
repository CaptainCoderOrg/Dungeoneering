-- Define an array of small items found in a furniture store
local items = {
    "Ottoman",
    "Bookcase",
    "Bed",
    "Recliner",
    "Sideboard",
    "Dresser",
    "Credenza",
    "Nightstand",
    "Couch",
    "Loveseat",
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
local min = math.random(6, 12)
local range = math.random(6, 12)

local success = context.GetVariable("final-0") and
  context.GetVariable("final-1") and
  context.GetVariable("final-2") and
  context.GetVariable("final-3")

context.GiveWeapon(name, min, min + range)
