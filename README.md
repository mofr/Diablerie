# Diablerie

[![Windows Build Status](http://diablerie.zond.org/build/image/?target=win64)](http://diablerie.zond.org/download/?target=win64)

[![Linux Build Status](http://diablerie.zond.org/build/image/?target=linux)](http://diablerie.zond.org/download/?target=linux)

![alt tag](https://raw.github.com/mofr/Diablerie/master/Screenshots/rogue_camp.png)

This is a recreation of Diablo 2 game from Blizzard.

The game is at a very early stage of development - work in progress.

No commercial use is intended. All images and sounds used are from Diablo 2 and are property of the original game creators.

# How to run the game

Requirements:
* Download latest version of Diablerie ([Windows](http://diablerie.zond.org/download/?target=win64), [Linux](http://diablerie.zond.org/download/?target=linux))
* Copy following files from Diablo 2 LOD v1.14 to the Diablerie directory:
  - d2exp.mpq
  - d2data.mpq
  - d2char.mpq
  - d2sfx.mpq (optional)
  - d2music.mpq (optional)
  - d2xMusic.mpq (optional)
  - d2xtalk.mpq (optional)
  - d2speech.mpq (optional)
* (Windows-only) Install [Microsoft Visual C++ Redistributable](https://www.microsoft.com/en-us/download/details.aspx?id=52685)
* Run Diablerie

# How to build and run from sources:

Requirements:
- Unity 2017.2.0f3 (other versions are not tested)
- Diablo 2 Lord of Destruction v1.14

Steps:
* git clone https://github.com/mofr/Diablerie.git
* Copy following files from Diablo 2 LOD v1.14 to the Diablerie directory:
  - d2exp.mpq
  - d2data.mpq
  - d2char.mpq
  - d2sfx.mpq (optional)
  - d2music.mpq (optional)
  - d2xMusic.mpq (optional)
  - d2xtalk.mpq (optional)
  - d2speech.mpq (optional)
* Run Unity Editor and open Diablerie folder as a project
* In Assets folder open `Scenes/MainMenu.scene` file
* Press `Play`

# Credits

Paul Siramy - ds1edit editor, great thanks for source code. Very helpful and detailed tutorials related to diablo 2 internals.

Bilian Belichev - DCC file format documentation and sample decoder.

Ladislav Zezula - [StormLib](https://github.com/ladislav-zezula/StormLib) 

[Phrozen Keep](http://d2mods.info) - A lot of various information about diablo 2 files.
