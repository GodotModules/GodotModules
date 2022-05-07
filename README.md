# Godot Modules
## About
A collection of useful modules and scripts for C# Godot games.

## Why make this?
I was thinking to myself, I want to make a bullet hell game, but I am also going to be making more then just one game in the future. I do not want to redo the same things over again. If I want multiplayer, I can just grab it from here. If I want a modloader, I can find it here. That is the motivation behind this project.

## Modules
### Core
In-game console on pressing F12, supports custom commands, useful for in-game testing / debugging.

![image](https://user-images.githubusercontent.com/6277739/166569933-de699808-6de9-4f7f-ac90-1a8ae460e262.png)

There are also popup error / message windows and the bottom right corner of the screen shows a small red box which notifies you of any errors with the total error count every second.

### [ModLoader](https://github.com/valkyrienyanko/GodotModules/blob/main/.github/MOD_LOADER.md)
![image](https://user-images.githubusercontent.com/6277739/162651881-b8f98aa5-da2a-4499-b4dd-737a64dec4a9.png)  

### [Netcode](https://github.com/valkyrienyanko/GodotModules/blob/main/.github/NETCODE.md)
![image](https://user-images.githubusercontent.com/6277739/164528687-8ce3891f-2aa2-4c43-b9d2-404620aefad2.png)
![image](https://user-images.githubusercontent.com/6277739/164519290-fcd96048-3267-4278-bbd9-34bd7c0a86c0.png)
![image](https://user-images.githubusercontent.com/6277739/164519339-a23cc3be-29dd-4df8-ad3b-e975508f5ec8.png)

https://user-images.githubusercontent.com/6277739/165597959-cb42938a-d680-45ec-99f0-d2ba4495a534.mp4

### Tech Tree (coming soon)
Tech tree where nodes in tree are positioned automatically via script

### Options
![image](https://user-images.githubusercontent.com/6277739/163117944-e350b70c-aaaa-426f-8719-3c28648d5747.png)  

### [Helper Scripts](https://github.com/valkyrienyanko/GodotModules/blob/main/.github/UTILITY_SCRIPTS.md)

## Contributing
There are 3 types of problems I always run into while working on this
- Tasks are fired and forgotten (wanted behavior) but are not cancelled at proper times causing all sorts of problems
- Duplicate player id added to lobby player dictionary
- Static variables not being reset for re-use

Tasks are hard to manage, you cancel them with a cancellation token but it's hard to keep track if you cancel them twice or dispose them twice and get a disposed already exception.

With the duplicate player joining it's because their joining ID is 0 just like the host id is 0. The joining ID should be 1. Usually this is because I forgot to write or read a byte but that dosen't seem to be the case this time but I might be wrong I do not know.

Static variables are a mess, I wish I never used them to begin with.

See [CONTRIBUTING.md](https://github.com/valkyrienyanko/GodotModules/blob/main/.github/CONTRIBUTING.md)

## Credit
### Programming
Thank you to [LazerGoat](https://github.com/LazerGoat) for pointing out bugs here and there

### Testers
Thank you to the following testers.

- LazerGoat [[GitHub]](https://github.com/LazerGoat)
- SCUDSTORM [[Twitch]](https://www.twitch.tv/perezdispenser)
