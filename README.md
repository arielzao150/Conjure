# Welcome to Conjure
### Introduction
Conjure is a console application developed by arielzao150 in order to export custom Magic The Gathering decks to Tabletop Simulator (TTS).
Mainly driven the Coronavirus, I decided to create this program in order to play with custom cards in TTS.
### How It Works
Conjure gets all the file in the Cockatrice format (also one of the export options in [mtg.design](mtg.design)), uploads all of them to [imgbb](https://imgbb.com/) and creates a file for TTS.
All you need is a folder with all the contents of your deck and an API key from imgbb.
After you have that, copy the executable to the folder that contains the deck folder. The folder should look something like this:

 - EDH
	 - EDH
		 - Images
	 - EDH.xml
 - Conjure.exe

After that run the command:

    Conjure -set [INSERT FOLDER NAME] -key [INSERT API KEY]

And there you go. If you did everything correct, you should see a .json file, ready to be used in Tabletop Simulator.
I advise you to do all of this in the C:\Users\[USER]\Documents\My Games\Tabletop Simulator\Saves\Saved Objects folder, that way you won't even need to copy and paste the JSON file.

## Custom Cardback

If you want your cards to have a custom cardback, all you need to do is add a "cardback.jpg" on the same folder of your XML file. Conjure will handle the rest.

That's it, I hope you enjoy.
Happy conjuring.
