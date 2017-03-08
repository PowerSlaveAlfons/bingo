# bingo
A bingo board generator.  

Mostly just rewriting Josh's JS web based Bingo Tool and rewriting it in C# to be application based, so global hotkeys can be used. (Important for when other windows need to be in focus.)  

Nothing too fancy


How to use:
Download zip
Make sure there's a goals folder next to Bongo.exe , I didn't test what happens if there isn't (probably a crash.)
Inside the goals folder is a folder titled whatever the goals should be titled (game name, probably).
Inside there should be a goals.xml file. If there isn't, the tool will crash trying to load it as I didn't put in a failsafe yet.

No configurable hotkeys yet, use Ctrl + IJKL for navigating now, Ctrl + UO for color changing, and Ctrl + M for unhiding the board. (Or use the mouse if you feel like alt tabbing.)


Features I want to add but might not:
Displaying goals version + some data on screen when loaded to help identify differences.
Goal antisynergy a la Joshimuz.
Configurable hotkeys, as manetioned earlier
Make this thing look pretty
Making it resizable souds useful too.
Support for goal length
Support for goal amount randomization. (37 hidden packages)
Maybe add an in-app goal creator so people who can't read XML can create goals.
Spectator mode
Export to file (or rather import, for those custom made bingo sheets)
