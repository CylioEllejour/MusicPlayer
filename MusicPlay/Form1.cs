using System;
using System.Windows.Forms;
using NAudio.Vorbis;
using NAudio.Wave;
using WMPLib;
using System.IO; // 用于MP3播放

namespace MusicPlay
{
    public partial class Form1 : Form
    {
        WindowsMediaPlayer player = new WindowsMediaPlayer();
        private IWavePlayer waveOutDevice;
        private AudioFileReader audioFileReader;
        private VorbisWaveReader vorbisWaveReader; // 添加这一行

        public Form1()
        {
            InitializeComponent();

            timer1.Interval = 1000; // Set the timer interval to 1 second.
            timer1.Tick += timer1_Tick; // Attach the Tick event handler.
            timer1.Start(); // Start the timer.
        }

        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.volume = 50;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 获取选中的歌曲
            if (listBox1.SelectedItem != null)
            {
                string selectedSong = listBox1.SelectedItem.ToString();

                // 检查歌曲文件是否存在
                if (File.Exists(selectedSong))
                {
                    if (Path.GetExtension(selectedSong) == ".ogg")
                    {
                        // 停止当前正在播放的歌曲
                        axWindowsMediaPlayer1.Ctlcontrols.stop();
                        if (waveOutDevice != null)
                        {
                            waveOutDevice.Stop();
                            waveOutDevice.Dispose();
                            waveOutDevice = null;
                        }
                        if (vorbisWaveReader != null)
                        {
                            vorbisWaveReader.Dispose();
                            vorbisWaveReader = null;
                        }

                        // 清理播放状态
                        axWindowsMediaPlayer1.URL = string.Empty;

                        // 使用 NAudio.Vorbis 来播放 OGG 文件
                        vorbisWaveReader = new NAudio.Vorbis.VorbisWaveReader(selectedSong);
                        waveOutDevice = new WaveOut();
                        waveOutDevice.Init(vorbisWaveReader);
                        waveOutDevice.Play();
                    }
                    else
                    {
                        // 停止当前正在播放的歌曲
                        axWindowsMediaPlayer1.Ctlcontrols.stop();

                        if (waveOutDevice != null)
                        {
                            waveOutDevice.Stop();
                            waveOutDevice.Dispose();
                            waveOutDevice = null;
                        }
                        if (vorbisWaveReader != null)
                        {
                            vorbisWaveReader.Dispose();
                            vorbisWaveReader = null;
                        }

                        // 清理播放状态
                        axWindowsMediaPlayer1.URL = string.Empty;

                        // 设置播放器的URL为选中的歌曲路径
                        axWindowsMediaPlayer1.URL = selectedSong;
                        axWindowsMediaPlayer1.Ctlcontrols.play();
                    }
                }
                else
                {
                    // 如果歌曲文件不存在，显示错误信息
                    MessageBox.Show($"The file {selectedSong} does not exist.");
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void next_Click(object sender, EventArgs e)
        {
            // 获取当前选中的歌曲的索引
            int currentIndex = listBox1.SelectedIndex;

            // 计算下一首歌的索引
            int nextIndex = (currentIndex + 1) % listBox1.Items.Count;

            // 选中下一首歌
            listBox1.SelectedIndex = nextIndex;

            // 获取选中的歌曲
            string nextSong = listBox1.SelectedItem.ToString();

            // 检查歌曲文件是否存在
            if (File.Exists(nextSong))
            {
                if (Path.GetExtension(nextSong) == ".ogg")
                {
                    if (waveOutDevice != null)
                    {
                        waveOutDevice.Stop();
                        waveOutDevice.Dispose();
                        waveOutDevice = null;
                    }
                    if (vorbisWaveReader != null)
                    {
                        vorbisWaveReader.Dispose();
                        vorbisWaveReader = null;
                    }

                    // 使用 NAudio.Vorbis 来播放 OGG 文件
                    vorbisWaveReader = new NAudio.Vorbis.VorbisWaveReader(nextSong);
                    waveOutDevice = new WaveOut();
                    waveOutDevice.Init(vorbisWaveReader);
                    waveOutDevice.Play();
                }
                else
                {
                    // 停止当前正在播放的歌曲
                    axWindowsMediaPlayer1.Ctlcontrols.stop();

                    // 设置播放器的URL为选中的歌曲路径，并播放
                    axWindowsMediaPlayer1.URL = nextSong;
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                }
            }
            else
            {
                // 如果歌曲文件不存在，显示错误信息
                MessageBox.Show($"The file {nextSong} does not exist.");
            }
        }
        private void pause_Click(object sender, EventArgs e)
        {
            // 检查播放器的播放状态
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                // 如果当前正在播放音乐，就暂停播放
                axWindowsMediaPlayer1.Ctlcontrols.pause();
            }
            else if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPaused)
            {
                // 如果当前音乐已经暂停，就恢复播放
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
            else if (waveOutDevice != null && waveOutDevice.PlaybackState == PlaybackState.Playing)
            {
                // 如果当前正在播放 OGG 音乐，就暂停播放
                waveOutDevice.Pause();
            }
            else if (waveOutDevice != null && waveOutDevice.PlaybackState == PlaybackState.Paused)
            {
                // 如果当前 OGG 音乐已经暂停，就恢复播放
                waveOutDevice.Play();
            }
        }
        private void select_Click(object sender, EventArgs e)
        {
            // 创建一个文件选择对话框
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 设置文件选择对话框的标题
            openFileDialog.Title = "Select a song";

            // 设置文件选择对话框的初始目录
            openFileDialog.InitialDirectory = @"C:\";

            // 设置文件选择对话框的过滤器，只显示 MP3 和 WAV 文件
            openFileDialog.Filter = "MP3 Files|*.mp3|WAV Files|*.wav|OGG Files|*.ogg";

            // 显示文件选择对话框，如果用户选择了一个文件，就将这个文件添加到 listBox1 中
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

                // 停止当前正在播放的歌曲
                axWindowsMediaPlayer1.Ctlcontrols.stop();
                if (waveOutDevice != null)
                {
                    waveOutDevice.Stop();
                    waveOutDevice.Dispose();
                    waveOutDevice = null;
                }
                if (vorbisWaveReader != null)
                {
                    vorbisWaveReader.Dispose();
                    vorbisWaveReader = null;
                }

                listBox1.Items.Add(openFileDialog.FileName);
                axWindowsMediaPlayer1.URL = openFileDialog.FileName;

                // 设置 TrackBar 控件的 Minimum 和 Maximum 属性
                trackBar1.Minimum = 0;
                trackBar1.Maximum = 100;

                // 设置 progress_trackBar 控件的 Minimum 和 Maximum 属性
                trackBar2.Minimum = 0;
                trackBar2.Maximum = (int)axWindowsMediaPlayer1.currentMedia.duration;
            }
        }

        private void select_ogg_Click(object sender, EventArgs e)
        {
            // 创建一个文件选择对话框
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 设置文件选择对话框的标题
            openFileDialog.Title = "Select an OGG file";

            // 设置文件选择对话框的初始目录
            openFileDialog.InitialDirectory = @"C:\";

            // 设置文件选择对话框的过滤器，只显示 OGG 文件
            openFileDialog.Filter = "OGG Files|*.ogg";

            // 显示文件选择对话框，如果用户选择了一个文件，就将这个文件添加到 listBox2 中
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                listBox1.Items.Add(openFileDialog.FileName);

                // 停止当前正在播放的歌曲
                axWindowsMediaPlayer1.Ctlcontrols.stop();
                if (waveOutDevice != null)
                {
                    waveOutDevice.Stop();
                    waveOutDevice.Dispose();
                    waveOutDevice = null;
                }
                if (vorbisWaveReader != null)
                {
                    vorbisWaveReader.Dispose();
                    vorbisWaveReader = null;
                }

                // 使用 NAudio.Vorbis 来播放 OGG 文件
                vorbisWaveReader = new NAudio.Vorbis.VorbisWaveReader(openFileDialog.FileName);
                waveOutDevice = new WaveOut();
                waveOutDevice.Init(vorbisWaveReader);
                waveOutDevice.Play();
            }
        }

        private void volume_trackBar_Scroll(object sender, EventArgs e)
        {
            // 将 WindowsMediaPlayer 控件的音量设置为滑动条的值
            axWindowsMediaPlayer1.settings.volume = trackBar1.Value;
            // 将 WaveOutDevice 控件的音量设置为滑动条的值
            if (waveOutDevice != null)
            {
                waveOutDevice.Volume = trackBar1.Value / 100f;
            }
        }

        // 在滑动条的 Scroll 事件处理函数中，将 WindowsMediaPlayer 控件的播放位置设置为滑动条的值
        private void progress_trackBar_Scroll(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                axWindowsMediaPlayer1.Ctlcontrols.currentPosition = trackBar2.Value;
            }
            else if (waveOutDevice != null && waveOutDevice.PlaybackState == PlaybackState.Playing)
            {
                vorbisWaveReader.CurrentTime = TimeSpan.FromSeconds(trackBar2.Value);
            }
        }

        // 使用一个 Timer 控件定期更新滑动条的值
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                trackBar2.Maximum = (int)axWindowsMediaPlayer1.currentMedia.duration;
                trackBar2.Value = (int)axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            }
            else if (waveOutDevice != null && waveOutDevice.PlaybackState == PlaybackState.Playing)
            {
                trackBar2.Maximum = (int)vorbisWaveReader.TotalTime.TotalSeconds;
                trackBar2.Value = (int)vorbisWaveReader.CurrentTime.TotalSeconds;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void Previous_Click(object sender, EventArgs e)
        {
            // 获取当前选中的歌曲的索引
            int currentIndex = listBox1.SelectedIndex;

            // 计算上一首歌的索引
            int previousIndex = (currentIndex - 1 + listBox1.Items.Count) % listBox1.Items.Count;

            // 选中上一首歌
            listBox1.SelectedIndex = previousIndex;

            // 获取选中的歌曲
            string previousSong = listBox1.SelectedItem.ToString();

            // 检查歌曲文件是否存在
            if (File.Exists(previousSong))
            {
                if (Path.GetExtension(previousSong) == ".ogg")
                {
                    if (waveOutDevice != null)
                    {
                        waveOutDevice.Stop();
                        waveOutDevice.Dispose();
                        waveOutDevice = null;
                    }
                    if (vorbisWaveReader != null)
                    {
                        vorbisWaveReader.Dispose();
                        vorbisWaveReader = null;
                    }

                    // 使用 NAudio.Vorbis 来播放 OGG 文件
                    vorbisWaveReader = new NAudio.Vorbis.VorbisWaveReader(previousSong);
                    waveOutDevice = new WaveOut();
                    waveOutDevice.Init(vorbisWaveReader);
                    waveOutDevice.Play();
                }
                else
                {
                    // 停止当前正在播放的歌曲
                    axWindowsMediaPlayer1.Ctlcontrols.stop();

                    // 设置播放器的URL为选中的歌曲路径，并播放
                    axWindowsMediaPlayer1.URL = previousSong;
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                }
            }
            else
            {
                // 如果歌曲文件不存在，显示错误信息
                MessageBox.Show($"The file {previousSong} does not exist.");
            }
        }
    }
}
