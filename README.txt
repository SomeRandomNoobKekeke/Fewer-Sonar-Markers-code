Mod allows you to hide some sonar Markers
Mod is serverside now

You can hide all marks of some type, in some specific mission, in some specific positions (e.g. cave), and on some type of sonar

Settings are stored in "barotrauma folder"\ModSettings\Fewer Sonar Markers\Settings.json
You can edit it with any text editor or with commands

Mod adds some commands: sm, sm_load, sm_save debugmissions
debugmissions shows you what type of missions you are on and what are their positionTypes if they have them

sm - mega command, it's fully tabable, usage:
sm easy, sm hard - load presets
sm_load [filename], sm_save [filename]- settings, settings are autosaved on each change so they are pretty useless
sm - enable / disable mod
sm [hide, reveal] [missiontype, all, labels, caves, minerals, outposts, submarines, aitargets] [positionType, any] onWhichSonar - hide / reveal something specific

Note, in multiplayer settings are synced between clients, only host or player with permissions 'all' can set them


This version is precompiled for faster load time and smoother JSON usage
If you are afraid of dll, or want to play with source code you can delete bin folder and move Client folder from Source code to CSharp

Source code might spit out json related errors, it's normal, just recompile it 
it's because json not included in main game so on first attempt to use it you get an error, on second it's gets lazyloaded