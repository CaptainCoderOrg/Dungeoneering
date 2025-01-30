local first = Dialogue([[As you turn the corner, you realize what is causing that horrific smell. Before you is what can only be described as a Sentient Swedish Meatball!]])
local tldr = Dialogue([[So... you want to skip the story? Are you sure?]])
tldr.AddOption(ContinueDialogueOption('Go back', first))
local bulletpoints = Dialogue([[* This guys is a cosmic meatball named Eye Key Uh
* Zooperdan, in his thirst for the best dungeon crawler made a cursed deal with Eye Key Uh
* Zooperdan is now trapped in Eye Key Uh's cosmic furniture store
* Find the Meatballs... destroy them... get home!]])
bulletpoints.AddOption(RunScriptDialogueOption('Got it!', 'begin-quest.lua'))
tldr.AddOption(ContinueDialogueOption('Give Me the Bullet Points', bulletpoints))


local second = Dialogue([['Good. You're finally awake.', the Meatball speaks. 'Now, it is time for you to fulfill your end of the deal.']])
local third = Dialogue([['Have you already forgotten the deal you made with me? Eye Key Uh? Lord Meatball of the Cosmos!']])
second.AddOption(ContinueDialogueOption('What deal?', third))


local fourth = Dialogue([[Suddenly, it all comes rushing back... It was the week of Dungeon Crawler Jam 2024 and Zooperdan began grinding away playing thousands of entries determined to rate them all.]])
third.AddOption(ContinueDialogueOption('This sounds serious', fourth))
third.AddOption(ContinueDialogueOption('tl;dr', tldr))

local fifth = Dialogue([[Minutes turned into hours... hours turned into days... days turned into weeks... ]])
fourth.AddOption(ContinueDialogueOption('And then?', fifth))
fourth.AddOption(ContinueDialogueOption('tl;dr', tldr))

local sixth = Dialogue([[As time wore on so did Zooperdan's mind until he began to hear a voice calling to him.]])
fifth.AddOption(ContinueDialogueOption('Voice?', sixth))
fifth.AddOption(ContinueDialogueOption('tl;dr', tldr))

local seventh = Dialogue([['Zooperdan, my star-dusted friend. I have what you seek... the Ultimate Dungeon Crawler. And I can give it to you...', the voice tempted him.]])
sixth.AddOption(ContinueDialogueOption('Yeah right!', seventh))
sixth.AddOption(ContinueDialogueOption('tl;dr', tldr))

local eighth = Dialogue([[With his mind worn down, he was unable to resist the call, 'Anything... just give it to me.']])
seventh.AddOption(ContinueDialogueOption('Continue', eighth))
seventh.AddOption(ContinueDialogueOption('tl;dr', tldr))

local ninth = Dialogue([[A heavy wet mist of Swedish Meatball air surrounded Zooperdan and he was enveloped in darkness... and now he is here.]])
eighth.AddOption(ContinueDialogueOption('More?', ninth))
eighth.AddOption(ContinueDialogueOption('tl;dr', tldr))

local ten = Dialogue([['Prepare yourself for my Cosmic Furniture Store! Within you will find my meatball army. If you can destroy them all, you can go home. But, if you fail, you will become my next meatball.']])
ninth.AddOption(ContinueDialogueOption('More?', ten))
ninth.AddOption(ContinueDialogueOption('tl;dr', tldr))

ten.AddOption(RunScriptDialogueOption("I'm Ready", 'begin-quest.lua'))
ten.AddOption(ContinueDialogueOption('tl;dr', tldr))




first.AddOption(ContinueDialogueOption('Continue', second))
first.AddOption(ContinueDialogueOption('tl;dr', tldr))
context.ShowDialogue(first)