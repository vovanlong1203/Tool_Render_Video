using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;


namespace Tool_Render_Video
{
    public class VideoManager
    {
        public string InputVideoPath { get;  set; }
        public string OutputVideoPath { get;  set; }

        public VideoManager()
        {
            Xabe.FFmpeg.FFmpeg.SetExecutablesPath(@"E:\ffmpeg\ffmpeg\bin");
        }

        // Phương thức chọn video
        public bool SelectInputVideo()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Video Files|*.mp4;*.avi;*.mov;*.mkv";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    InputVideoPath = openFileDialog.FileName;
                    return true;
                }
                return false;
            }
        }

        public async Task TrimVideoAsync(TimeSpan startTime, TimeSpan duration)
        {
            if (string.IsNullOrEmpty(InputVideoPath))
            {
                MessageBox.Show("Chưa chọn video nguồn!");
                return;
            }
            try
            {
                OutputVideoPath = Path.Combine(
                    Path.GetDirectoryName(InputVideoPath),
                    $"Trimmed_{Path.GetFileName(InputVideoPath)}"
                );

                IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(InputVideoPath);
                await FFmpeg.Conversions.New()
                    .AddStream(mediaInfo.VideoStreams.FirstOrDefault())
                    .AddStream(mediaInfo.AudioStreams.FirstOrDefault())
                    .SetSeek(startTime)
                    .SetOutputTime(duration)
                    .SetOutput(OutputVideoPath)
                    .Start();

                MessageBox.Show("Cắt video thành công!");
            } catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cắt video: {ex.Message}");
            }
        }

        public async Task ChangeVideoSpeedAsync(double speed)
        {
            if (string.IsNullOrEmpty(InputVideoPath))
            {
                MessageBox.Show("Chưa chọn video nguồn!");
                return;
            }

            // Adjust speed calculation to work consistently
            speed = speed switch
            {
                50 => 1.0,    // Normal speed
                < 50 => speed / 50.0,  // Slower speeds
                > 50 => speed / 50.0,  // Faster speeds
                _ => 1.0
            };

            // Ensure speed is within reasonable bounds
            speed = Math.Clamp(speed, 0.5, 2.0);

            try
            {
                IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(InputVideoPath);
                OutputVideoPath = Path.Combine(
                    Path.GetDirectoryName(InputVideoPath),
                    $"Speed_{speed}x_{Path.GetFileName(InputVideoPath)}"
                );

                var conversion = Xabe.FFmpeg.FFmpeg.Conversions.New()
                    .AddStream(mediaInfo.VideoStreams.FirstOrDefault())
                    .AddStream(mediaInfo.AudioStreams.FirstOrDefault());

                // Video speed adjustment
                conversion.AddParameter($"-filter:v \"setpts={1 / speed}*PTS\"");

                // Audio speed adjustment with improved handling
                if (speed <= 2)
                {
                    conversion.AddParameter($"-filter:a \"atempo={speed}\"");
                }
                else
                {
                    // For speeds > 2, cascade atempo filters
                    double remainingSpeed = speed;
                    List<string> tempoFilters = new List<string>();
                    while (remainingSpeed > 2)
                    {
                        tempoFilters.Add("atempo=2");
                        remainingSpeed /= 2;
                    }
                    tempoFilters.Add($"atempo={remainingSpeed}");
                    conversion.AddParameter($"-filter:a \"{string.Join(',', tempoFilters)}\"");
                }

                // Execute conversion
                await conversion
                    .SetOutput(OutputVideoPath)
                    .Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thay đổi tốc độ: {ex.Message}");
            }
        }
    }
}

