# YouTube-Search

_PowerShell script that helps you find Youtube videos (without API) and open them using mpv/yt-dlp._

My modest attempt to port [ytfzf](https://github.com/pystardust/ytfzf) to Windows. 

> Really raw version.

## Showcase

![](https://github.com/HenryKovalevsky/yts/raw/master/assets/yts.gif)

## Prerequisites

- [`PowerShell 7+`](https://github.com/PowerShell/PowerShell)
- [`fzf`](https://github.com/junegunn/fzf) (menu selection screen)
- [`curl`](https://curl.se/)
- [`jq`](https://jqlang.github.io/jq/)
- [`chafa`](https://hpjansson.org/chafa/) (thumbnails viewer)
- [`mpv`](https://mpv.io/) (video and audio player)
- [`yt-dlp`](https://github.com/yt-dlp/yt-dlp)
- Terminal with the sixel graphics support (e.g. [`Windows Terminal Preview 1.22+`](https://aka.ms/terminal-preview))

> You can easily install all the dependencies with [`Scoop`](https://scoop.sh/).

```pwsh
scoop install fzf curl jq chafa mpv yt-dlp
```

## Installation

```pwsh
scoop install https://raw.githubusercontent.com/HenryKovalevsky/yts/refs/heads/master/scoop/yts.json
```

Or copy [`yts.ps1`](https://github.com/HenryKovalevsky/yts/raw/master/yts.ps1) and add to PATH.

## Usage

> Search video:

```pwsh
yts <search>
```

> To run `mpv` with `--no-video` flag:

```pwsh
$ yts -a <search>
```
