banner goes here eventually
# FRESHMusicPlayer-Core
audio library abstraction library - designed to be used for the FRESHMusicPlayer project but can be used on anything :)
- [**Support/Discussion Discord Server**](https://discord.gg/mFGFT8K)
## Features
- Abstracts music playback into a simple API
- todo: add more stuff here
## Usage
```
using FRESHMusicPlayer;

var player = new Player();
string path = "Can be a file path, or a URL to a network stream";
player.PlayMusic(path);  // Clear the queue, add the track to the queue, and play
// or
player.Queue.Add(path);
player.PlayMusic(); // Plays through the queue
```
### Platforms
**Windows** - NAudio is required for audio playback   
**Other platforms** - You'll need to include the [VLC audio plugin](https://github.com/DeclanHoare/FmpVlcBackend) in the output directory of your app for audio playback to work. The user will also need to have a global installation of VLC available. Hoping to replace this with something lighterweight in the future.
[**Documentation (WIP)**]()
## Projects that use FMP Core
- [**FRESHMusicPlayer**](https://github.com/royce551/freshmusicplayer)
- [**FRESHMusicPlayer-WinForms**](https://github.com/royce551/freshmusicplayer-wpfui)
