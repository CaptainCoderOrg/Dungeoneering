---@meta

North = 0
East = 1
South = 2
West = 3

None = 0
Solid = 1
Door = 2
Secret = 3


Hit = 0
Die = 1
Footstep = 2
Eat = 3
Miss = 4
Rest = 5
Guard = 6
Cheat = 7
LevelUp = 8

---@class context
context = {}

---@class PlayerView
context.PlayerView = {}

context.PlayerView.Position = {}
context.PlayerView.Position.X = 0
context.PlayerView.Position.Y = 0
context.PlayerView.Facing = North

--- Sets the player's position
function context.SetPlayerPosition(x, y) end

--- Sets the player's position and facing.
function context.SetPlayerView(x, y, facing) end

--- Sets the player's facing. Use: North, East, South, or West
function context.SetPlayerFacing(facing) end

--- Retrieves the type of wall the player is currently facing: Solid, Door,
--  Secret, or None
function context.GetWall() end

--- Retrieves the type of wall at the specified location and facing.
function context.GetWallAt(x, y, facing) end

--- Sets the wall the player is currently facing. Use: Solid, Door, Secret, or None
function context.SetWall() end

--- Sets the wall at the specified position and facing to the specified type.
function context.SetWallAt(x, y, facing, type) end

--- Writes a debug message to the console
function context.Debug(message) end

--- Writes information to the player.
function context.WriteInfo(message) end

--- Sets a global variable that can be used between scripts
function context.SetVariable(name, value) end

--- Retrieves a global variable or nil if no value was previously set
function context.GetVariable(name) end

--- Teleports the player to a new dungeon and sets their position and facing
function context.ChangeDungeon(name, x, y, facing) end

--- Loads a dungeon crawler from the specified url
function context.LoadCrawlerFromURL(url) end

--- Sets the texture of the wall the player is currently facing
function context.SetWallTexture(textureName) end

--- Sets the texture of the wall at the specified position
function context.SetWallTextureAt(x, y, facing, textureName) end

--- Retrieves the texture of the wall the player is facing
function context.GetWallTexture() end

--- Retrieves the texture of the wall at the specified position
function context.GetWallAtTexture(x, y, facing, textureName) end

--- Runs the specified script
function context.RunScript(scriptName) end

--- Shows the specified dialogue to the player
function context.ShowDialogue(dialogue) end

-- Initializes combat
function context.StartCombat(mapSetup, onWin, onGiveUp) end

-- returns the number of times the player has visted this location
function context.VisitedLocationCount() end

-- returns true if this is the first time the player has enterd this position
function context.IsFirstVisit() end

-- Heals the party to full
function context.HealParty() end

-- Adds the specified party member to the party
function context.AddPartyMember(ix) end

-- Adds the specified party member to the party
function context.RemovePartyMember(ix) end

-- Moves the player back to their previous position
function context.PushBack() end

-- Give the party a weapon
function context.GiveWeapon(name, min, max) end

-- Play the specified sound effect
function context.PlaySound(sound) end

-- If the player is in combat, exits combat
function context.ExitCombat() end

-- Show Credits
function context.ShowCredits() end

--- Constructs a new dialogue with the specified text
---@param text any
function Dialogue(text)
    local dialogue = {}
    --- Add an option to this dialogue
    function dialogue.AddOption(option) end

    return dialogue
end

--- Constructs an option that when selected closes the Dialogue
function CloseDialogueOption(label) end

--- Construts an option that when selected shows the specified dialogue
function ContinueDialogueOption(label, dialogue) end

--- Constructs an option that when selected executes the specified script
function RunScriptDialogueOption(label, scriptName) end
