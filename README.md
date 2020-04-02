# Diablerie

[![Windows Build Status](http://diablerie.zond.org/build/image/?target=win64)](http://diablerie.zond.org/download/?target=win64)

[![Linux Build Status](http://diablerie.zond.org/build/image/?target=linux)](http://diablerie.zond.org/download/?target=linux)

[![Code Status](https://www.codefactor.io/repository/github/liberodark/Diablerie/badge)](https://www.codefactor.io/repository/github/liberodark/Diablerie)

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
* Run Diablerie

# How to build and run from sources:

Requirements:
- Unity 2019.2.8f1 (other versions are not tested)
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

# In-game console

Press enter to open the in-game console.
Some commands to test:

Spawn Immortal King Set under the cursor `/spawn itemset immortal`

Spawn diablo under the cursor `/spawn diablo`

Spawn 100 skeletons under the cursor `/spawn skeleton1 100`

Go to act 1 `/act 1`

Go to act 2 `/act 2`

Go to act 3 `/act 3`

Go to act 4 `/act 4`

Go to act 5 `/act 5`

# FAQ

## What the goals of the Diablerie project?
From highest priority to lowest:
1. Implement all original Diablo II functionality. It will be cross-platform, easily run on modern systems in high resolution.
2. Have a good separation of the Game and the Engine, so that engine can be used separately to create Diablo-like games.
3. Create an original game in the setting of Diablo.

## Why Unity?
Unity is simple yet powerful game engine. It saves a lot of time for doing very basic stuff. There a lot of people who already familiar with Unity. Also it simple enough to dive into for new developers.
A lot of people have performance concerns about Unity. It shouldn't be an issue, and in case of serious bottlenecks it's possible to offload heavy computations to C++ libraries (as it's already done with MPQ loading code).
New Unity Data-Oriented Tech Stack is also available and provides great performance.

## Why C#
C# is popular, simple and performance enough language for today to make a game like Diablo. The main reason however is because using Unity implies using C#.

## Can I Play Diablerie without original Diablo II?
No, you can't. Original game resources are property of Blizzard and distributing it is not legal.

## Can I make mods for Diablerie?
Yes! This is one of the main goals of the project.

## Can I make my own game based on Diablerie engine?
Yes. The project consists of two parts: Engine and Game. One of the Diablerie goals is to have good separation of the Game (which is actually Diablo II remaster) and the Engine, which can be used to create Diablo II clones.

# Credits

Paul Siramy - ds1edit editor, great thanks for source code. Very helpful and detailed tutorials related to diablo 2 internals.

Bilian Belichev - DCC file format documentation and sample decoder.

Ladislav Zezula - [StormLib](https://github.com/ladislav-zezula/StormLib) 

[Phrozen Keep](http://d2mods.info) - A lot of various information about diablo 2 files.
