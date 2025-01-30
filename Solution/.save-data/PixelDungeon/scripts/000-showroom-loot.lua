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
local min = math.random(4, 8)
local range = math.random(4, 8)

local success = context.GetVariable("final-0") and
  context.GetVariable("final-1") and
  context.GetVariable("final-2") and
  context.GetVariable("final-3")
if not success then
    context.GiveWeapon(name, min, min + range)
else
    context.SetVariable("has-blue-key", true)
    local dialogue = Dialogue([[As the last meatball falls to the ground, Zooperdan let's out a cry of relief! The hold that Eye Key Uh had over him is broken.]])
    local more = Dialogue([[Searching through the pile of dead meat, you find a blue key!]])
    more.AddOption(CloseDialogueOption("Leave"))
    dialogue.AddOption(ContinueDialogueOption("Continue", more))
    context.ShowDialogue(dialogue);    
end

