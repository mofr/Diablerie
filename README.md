# Diablerie

[![Build Status](http://diablerie.zond.org/build/image/)](http://diablerie.zond.org/download/)

![alt tag](https://raw.github.com/mofr/Diablerie/master/Screenshots/rogue_camp.png)

This is a recreation of Diablo 2 game from Blizzard.

The game is at a very early stage of development - work in progress.

No commercial use is intended. All images and sounds used are from Diablo 2 and are property of the original game creators.

# How to run the game

Requirements:
* [Download latest version](http://diablerie.zond.org/download/) of Diablerie
* Put following files to the Diablerie directory from Diablo 2 LOD v1.14:
  - d2exp.mpq
  - d2data.mpq
  - d2char.mpq
  - d2sfx.mpq
  - d2music.mpq
  - d2xMusic.mpq

# How to build and run from sources:

Requirements:
- Unity 5.6.0f3 (other versions are not tested)
- Diablo 2 Lord of Destruction v1.14
- Diablerie source code

* git clone https://github.com/mofr/Diablerie.git
* Put following files to the Diablerie directory:
  - d2exp.mpq
  - d2data.mpq
  - d2char.mpq
  - d2sfx.mpq
  - d2music.mpq
  - d2xMusic.mpq
* Run Unity Editor and open Diablerie folder as a project
* In Assets folder open `Scenes/Scene1.scene` file
* Press `Play`

# Credits

Paul Siramy - ds1edit editor, great thanks for source code. Very helpful and detailed tutorials related to diablo 2 internals.

Bilian Belichev - DCC file format documentation and sample decoder.

Fabien BARBIER - CrystalMpq.

Foole (fooleau@gmail.com) - MpqHuffman and MpqWavCompression.

[Phrozen Keep](http://d2mods.info) - A lot of various information about diablo 2 files.
