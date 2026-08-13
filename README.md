# G3CONV
Multimedia converter for 3G phones

Converts picture/audio/video format to compatible formats used in 3G phones (i.e. those of Sony Ericsson).

Program works in Windows 2000 and newer. Requires .NET framework 3.5

Note: ffmpeg.exe executable must be in the same directory as G3CONV.exe (obtain the FFMPEG binary from the ffmpeg project). And input files in non-English characters are not supported by the program (you can convert them first by using UNI2C437 program).

<img width="1687" height="964" alt="G3" src="https://github.com/user-attachments/assets/78b998ec-dabc-4a08-96e8-cc939bebaaab" />

**Creating backup of your multimedia files prior converting is highly encouraged, as they might be overwritten/renamed/damaged during the conversion process.**

## Detailed information:

**Video format conversion settings:**

- Output format: 3GP
- Codec: libx264
- Maximal Resolution: 320x240, 24 fps
- Video Bitrate: variable 125kb/s
- Audio Sample Rate: 22050Hz
- Audio Bitrate: 32kb/s
- Output audio volume is set to half of the input
- Metadata are removed

FFMPEG command:

GWC.WriteLine("    {0} -%ISCUDA% -i \"{1}\" -c:v libx264 -profile:v baseline -filter_complex \"scale='iw*min(1,min(320/iw,240/ih))':-1:flags=lanczos,fps=24,pad=ceil(iw/4)*4:ceil(ih/4)*4\" -b:v 125k -minrate 100k -maxrate 150k -bufsize 1835k -map_metadata -1 -preset slow -pass 1 -an -f 3g2 NUL", FFMPEGPTH, FPTHNEW);
GWC.WriteLine("    {0} -%ISCUDA% -i \"{1}\" -c:v libx264 -profile:v baseline -filter_complex \"scale='iw*min(1,min(320/iw,240/ih))':-1:flags=lanczos,fps=24,pad=ceil(iw/4)*4:ceil(ih/4)*4\" -b:v 125k -minrate 100k -maxrate 150k -bufsize 1835k -map_metadata -1 -preset fast -pass 2 -c:a aac -strict experimental -b:a 32k -ar 22050 -filter:a \"volume=0.50\" -ac 1 \"{2}\"", FFMPEGPTH, FPTHNEW, FNEW);

**Audio format conversion settings:**

- Output format: mp3
- Codec: libmp3lame
- Audio Sample Rate: 22050Hz
- Audio Bitrate: 32kb/s
- Output audio volume is set to half of the input
- Metadata are removed

FFMPEG command:

GWC.WriteLine("   {0} -%ISCUDA% -i \"{1}\" -acodec libmp3lame -strict experimental -b:a 32k -ar 22050 -filter:a \"volume=0.50\" -map_metadata -1 -ac 1 \"{2}\"", FFMPEGPTH, FPTHNEW, FNEW);

**Picture format conversion settings:**

- Output format: jpg
- Pixel format: argb
- Maximal Resolution: 640x480
- Metadata are removed

FFMPEG command:

GWC.WriteLine("   {0} -%ISCUDA% -i \"{1}\" -filter_complex \"scale='iw*min(1,min(640/iw,480/ih))':-1:flags=lanczos,pad=ceil(iw/4)*4:ceil(ih/4)*4\" -pix_fmt argb -map_metadata -1 \"{2}\"", FFMPEGPTH, FPTHNEW, FNEW);
