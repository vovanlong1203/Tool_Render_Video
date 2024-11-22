using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Xabe.FFmpeg; // Thư viện FFmpeg dễ sử dụng

namespace Tool_Render_Video
{
    public partial class Form1 : Form
    {
/*        private string inputVideoPath = "";
        private string outputVideoPath = "";*/

        public readonly VideoManager _videoManager;
        public Form1()
        {
            InitializeComponent();
            InitializeUI();
            _videoManager = new VideoManager();
        }

        private void InitializeUI()
        {
            this.Text = "Video Edit & Render Tool";
            this.Size = new System.Drawing.Size(800, 600);

            Button btnSelectInput = new Button
            {
                Text = "Chon Video Nguon",
                Location = new System.Drawing.Point(20, 20),
                Width = 200
            };
            btnSelectInput.Click += BtnSelectInput_Click;

            this.Controls.Add(btnSelectInput);

            // Các nút điều chỉnh video
            string[] editOptions = { "Cắt Video", "Thay Đổi Tốc Độ", "Chỉnh Màu", "Thêm Hiệu Ứng" };
            for (int i = 0; i < editOptions.Length; i++)
            {
                Button btn = new Button
                {
                    Text = editOptions[i],
                    Location = new System.Drawing.Point(20, 60 + i * 40),
                    Width = 200
                };
                btn.Click += EditButton_Click;
                this.Controls.Add(btn);
            }

            // Thanh tiến trình render
            ProgressBar progressRender = new ProgressBar
            {
                Location = new System.Drawing.Point(250, 500),
                Width = 500,
                Minimum = 0,
                Maximum = 100
            };
            this.Controls.Add(progressRender);


            // Nút render video
            Button btnRender = new Button
            {
                Text = "Render Video",
                Location = new System.Drawing.Point(20, 500),
                Width = 200
            };
            btnRender.Click += BtnRender_Click;
            this.Controls.Add(btnRender);

        }

        private void ShowTrimVideoDialog()
        {
            Form trimForm = new Form
            {
                Text = "Cắt Video",
                Size = new System.Drawing.Size(400, 300)
            };

            // Các điều khiển để cắt video
            Label lblStart = new Label { Text = "Thời Gian Bắt Đầu (giây):", Location = new System.Drawing.Point(20, 20) };
            TextBox txtStart = new TextBox { Location = new System.Drawing.Point(200, 20), Width = 150 };

            Label lblEnd = new Label { Text = "Thời Gian Kết Thúc (giây):", Location = new System.Drawing.Point(20, 60) };
            TextBox txtEnd = new TextBox { Location = new System.Drawing.Point(200, 60), Width = 150 };

            Button btnTrim = new Button
            {
                Text = "Cắt",
                Location = new System.Drawing.Point(20, 100),
                Width = 200
            };
            btnTrim.Click += (s, e) => {
                // Logic cắt video
                MessageBox.Show("Chức năng cắt video đang được phát triển");
            };

            trimForm.Controls.AddRange(new Control[] { lblStart, txtStart, lblEnd, txtEnd, btnTrim });
            trimForm.ShowDialog();
        }

        private void ShowSpeedChangeDialog()
        {
            Form speedForm = new Form
            {
                Text = "Thay Đổi Tốc Độ Video",
                Size = new System.Drawing.Size(400, 300)
            };

            // Các điều khiển để thay đổi tốc độ
            TrackBar speedTrackBar = new TrackBar
            {
                Minimum = 1,
                Maximum = 100,
                Value = 1,
                Location = new System.Drawing.Point(20, 20),
                Width = 300
            };

            Label lblSpeed = new Label
            {
                Text = "Tốc Độ: 100%",
                Location = new System.Drawing.Point(20, 100),
                Width = 200
            };

            speedTrackBar.Scroll += (s, e) => {
                lblSpeed.Text = $"Tốc Độ: {speedTrackBar.Value}%";
            };

            Button btnApplySpeed = new Button
            {
                Text = "Áp Dụng",
                Location = new System.Drawing.Point(20, 150),
                Width = 200
            };
            btnApplySpeed.Click += async (s, e) => {
                // Logic thay đổi tốc độ
                await _videoManager.ChangeVideoSpeedAsync(speedTrackBar.Value);
                MessageBox.Show("Chức năng thay đổi tốc độ đang được phát triển");

            };

            speedForm.Controls.AddRange(new Control[] { speedTrackBar, lblSpeed, btnApplySpeed });
            speedForm.ShowDialog();
        }

        private void BtnRender_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_videoManager.InputVideoPath))
            {
                MessageBox.Show("Vui lòng chọn video nguồn!");
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "MP4 Video|*.mp4";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _videoManager.OutputVideoPath = saveFileDialog.FileName;
                    RenderVideo();
                }
            }
        }
        private void RenderVideo()
        {
            // Mã giả để render video
            MessageBox.Show($"Đang render video từ {_videoManager.InputVideoPath} sang {_videoManager.OutputVideoPath}");
        }

        private void BtnSelectInput_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Video Files|*.mp4;*.avi;*.mov;*.mkv";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _videoManager.InputVideoPath = openFileDialog.FileName;
                    MessageBox.Show($"Đã chọn video: {Path.GetFileName(_videoManager.InputVideoPath)}");
                }
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            switch (clickedButton.Text)
            {
                case "Cắt Video":
                    ShowTrimVideoDialog();
                    break;
                case "Thay Đổi Tốc Độ":
                    ShowSpeedChangeDialog();
                    break;
                    // Các chức năng chỉnh sửa khác
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
