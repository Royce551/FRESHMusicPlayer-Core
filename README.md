banner goes here eventually
# FRESHMusicPlayer-Core
Music Player component for Windows apps - designed to be used for the FRESHMusicPlayer project but can be used on anything :)
- [**Support/Discussion Discord Server**](https://discord.gg/mFGFT8K)
## Features
- Abstracts music playback into a simple API
- Provides a library that's shared between all FMP Core based apps
- Has built in support for integrations with services like Discord
## Usage
```
using FRESHMusicPlayer;

Player player = new Player();
Console.WriteLine("What file do you want to play?");
string path = Console.ReadLine();
player.AddQueue(path);  // Everything in FMP runs on a queue
player.PlayMusic();  // Play through the queue
```
[**Documentation (WIP)**]()
## Projects that use FMP Core
- [**FRESHMusicPlayer**](https://github.com/royce551/freshmusicplayer)
- [**FRESHMusicPlayer-WPFUI** (new, WPF frontend which will eventually replace the above)](https://github.com/royce551/freshmusicplayer-wpfui)
