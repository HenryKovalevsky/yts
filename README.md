# YouTube-Search

_A simple terminal app that helps you find Youtube videos (without API) and open them using mpv/yt-dlp._

My modest attempt to port [ytfzf](https://github.com/pystardust/ytfzf) to Windows. 

Made with [F#](https://fsharp.org) and [Thuja](https://github.com/HenryKovalevsky/Thuja). 

_For the raw powershell version see [this branch](https://github.com/HenryKovalevsky/yts/tree/fzf+pwsh)._

## Showcase

![](https://github.com/HenryKovalevsky/yts/raw/master/assets/yts.gif)

## Prerequisites

- [`PowerShell 7+`](https://github.com/PowerShell/PowerShell)
- [`chafa`](https://hpjansson.org/chafa/) (thumbnails viewer)
- [`mpv`](https://mpv.io/) (video and audio player)
- [`yt-dlp`](https://github.com/yt-dlp/yt-dlp)
- Terminal with the sixel graphics support (e.g. [`Windows Terminal Preview 1.22+`](https://aka.ms/terminal-preview))

> You can easily install all the dependencies with [`Scoop`](https://scoop.sh/).

```pwsh
scoop install chafa mpv yt-dlp
```

## Installation

```pwsh
scoop install https://raw.githubusercontent.com/HenryKovalevsky/yts/refs/heads/master/scoop/yts.json
```

## Usage

Search video:

```pwsh
yts <search>
```