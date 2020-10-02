Description:
This Mod allows you to customize the default carry weight formula.

Installtion
Download "carry weight customizer.dfmod" for your Operating System (windows, OSX, or Linux)
copy "carry weight customizer.dfmod" to your daggerfall_Data/StreamingAssets/Mods folder.
Start the game, and click on mods (lower left corner).  Find Carry Weight Customizer in the list of mods, and then ensure that it is enabled.  Click on Settings, and adjust to your taste.  Click on Save to save your settings, and then save and close to exit the mod section.  Click on play to play with the new settings.

Note: This will work with existing saves as well as new saves.


Uninstallation
Delete "carry weight customizer.dfmod" from daggerfall_Data/StreamingAssets/Mods.   If you only want to temporarily remove it, you can disable it in the mods section without removing the dfmod.


How the formula works:

By default, daggerfall calculates the carry weight as Strength * 1.5.   In Settings, you can change that multiplier to any integer value between 1 and 10.  You can also implement a modifier.  This setting will adjust the carry weight by a value based on the average of your athletic skills. (Climbing, Jumping, Running, and Swimming).   

You can choose either setting or both.  This is all adjusted through the settings button in mods section.

Note that you must enable the option that you would like.  If you only want to use the multiplier, then uncheck the Modify Carry Weight setting.  You are allowed to check or uncheck both.  If you uncheck both then the vanilla formula of 1.5 * strength is set.

Example of use:

Settings:
	Use Custom Strength Multipler set to True
	Multiplier = 2

	Use Modify Carry Weight set to True
	Multiplier = 2

	Player has Strength = 80, Climbing Skill = 10, Jumping Skill = 20, Running Skill = 12, and Swimming Skill = 6

	Player Carry Weight = Strength * 2 = 160
	Modifer = average of Athletic Skills * 2 = ( ( 10 + 20 + 12 + 6 ) / 4 ) * 2) = 24

	Total Carry Weight = 160 + 24 = 184


Later on in the game, Player has advanced in Strength and skills. Player has Strength = 120, Climbing Skill = 60, Jumping Skill = 80, Running Skill = 72, and Swimming Skill = 68


	Player Carry Weight = Strength * 2 = 240
	Modifer = average of Athletic Skills * 2 = ( ( 60 + 80 + 72 + 68 ) / 4 ) * 2) = 140

	Total Carry Weight = 240 + 140 = 380

Improving your Athletic skills improved your carry weight by more than 50%; thus advancing those skills have added benefit.


thanks
Asesino