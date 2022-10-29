# Space Invaders (Atari 800)

Challenge: Recreate a game from my childhood.  It was around the early 1980s when we got our first home computer, the Atari 800.  This computer was single-handedly responsible for my interest in video games, programming, and my later career in IT.  Space Invaders was one of the first games we had, and many hours were lost to this game.

This project is a recreation of that specific Atari 800 version.  It differs from the original version of Space Invaders in a number of ways, not least of which is the lack of bunkers/shields, but also the descending rocketship on the left hand side of the screen.  Extra lives are not awarded, nor does the game display the High Score.

Interesting Fact: Atari didn't allow the names of the programmers to appear in credits back then, but the programmer of this version, Rob Fulop, applied a clever approach to circumvent that.  Rotate your head 90 degrees to the right and you'll see that the bottom two rows of invaders create the letters "R" and "F" during their animation cycle.  

I have tried to recreate the original as closely as I could which involved considerable research.  I have used resources on YouTube (gameplay videos) and the emulated rom version (via Altirra) for reference.  I would like to thank Rob Fulop for his time and responding to my messages during the development of this clone.

Play for free here: https://robmeade.itch.io/atari-space-invaders

## Features

- Single player gameplay
- Game variations by means of game modes
- Increasing difficulty (rocketship lowers after each wave)
- Mystery ship for bonus points

## Assets
2D art assts were created by myself, using the above sources are reference.  The font was sourced online.  The sound effects were, regrettably, sampled from the Altirra emulated rom version.  

## Development

The project was created using Unity (2019.1.9f1).  Code was written using C#.  The project incorporates extensive _configuration_ variables to support the refinement of various aspects of the game, such as invader movement speed, player laser speed, points, and point modifiers.

## Further Development
Plans for further development include;

- Support for player two (pass-and-play)
- Game Modes to implement original Game Matrix 
	- Alternating slow and fast enemy laser beams
	- Fast enemy laser beams
	- Home-in enemy laser beams

![Game Manual](/Images/GameManual_Page9.PNG)

## Screenshots

Main Menu<br/>
![Main Menu](/Images/MainMenu.png)

Gameplay<br/>
![Gameplay](/Images/Gameplay.png)