using Microsoft.Web.WebView2.WinForms;

namespace EFTMap
{
    public partial class Form1 : Form
    {
        private static bool IsRun = false;
        private static readonly float SiteScale = 0.95f;
        private readonly string start = Path.Combine(Environment.CurrentDirectory, "start.mp3");


        private CancellationTokenSource? fileMonitorTokenSource;
        private readonly CancellationTokenSource? logMonitorTokenSource;
        private static WebView2 browser = new();
        private readonly NPlayer player = new();

        private static readonly string tarkovmarket = "https://tarkov-market.com/maps/";
        private static readonly string[] Map = ["ground-zero", "factory", "customs", "interchange", "woods", "shoreline", "lighthouse", "reserve", "streets", "lab"];
        private static readonly string EFTScreenshot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Escape from Tarkov\Screenshots");

        private readonly Monitor LogMonitor = new();
        internal static Monitor.GameState? GameState = null;


        public Form1()
        {
            InitializeComponent();

            browser = webView;

            StartPosition = FormStartPosition.Manual;
            Location = Config.Location.Get();
            CB_AutoSelect.Checked = Config.AutoSelect.Get();
            CB_AutoRemove.Checked = Config.AutoRemove.Get();
            
            webView.Size = ClientSize - new Size(webView.Location);
            tabControl1.Size = ClientSize - new Size(tabControl1.Location);

            comboBox1.BringToFront();
            LogMonitor.OnLog += OnLog;
            LogMonitor.OnGameStateChanged += OnGameState;
            LogMonitor.OnMapChanged += OnMapChanged;
            LogMonitor.StartMonitoring();
            StartFileMonitoring();

            #region NavigationCompleted (페이지 로딩후 자바스크립트 적용)
            browser.NavigationCompleted += (sender, e) =>
            {
                IsRun = true;
                Invoke(new Action(async () =>
                {
                    await Task.Delay(2500);
                    browser.SiteScale(SiteScale);
                    browser.InputEnable();
                }));
            };
            #endregion

            this.Resize += new EventHandler(Form_Resize);
        }

        #region Log 모니터링
        private void OnMapChanged(string map)
        {
            //Debug.WriteLine(map);
            if (IsRun && CB_AutoSelect.Checked)
            {
                if (map != null && map != "Labyrinth")
                {
                    Invoke(new Action(() =>
                    {
                        if (comboBox1.Text != map)
                        {
                            int index = comboBox1.FindStringExact(map);
                            if (index >= 0)
                            {
                                comboBox1.SelectedIndex = index;
                            }
                        }
                    }));
                }
            }
        }

        private void OnGameState(Monitor.GameState GameState)
        {
            if (IsRun)
            {
                if (GameState == Monitor.GameState.Start && Label_state.Text == "End")
                {
                    Invoke(new Action(() =>
                    {
                        Label_state.Text = "Start";
                        player.Mp3(start);
                    }));
                }
                if (GameState == Monitor.GameState.End && Label_state.Text == "Start")
                {
                    Invoke(new Action(() =>
                    {
                        Label_state.Text = "End";
                    }));
                }
            }
        }

        private void OnLog(string obj)
        {
            //Debug.WriteLine(obj);
        }
        #endregion


        private void Form_Resize(object? sender, EventArgs e)
        {
            webView.Size = ClientSize - new Size(webView.Location);
            tabControl1.Size = ClientSize - new Size(tabControl1.Location);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(Map);
            comboBox1.SelectedIndex = Config.MapIndex.Get();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeMap(comboBox1.SelectedIndex);
        }

        private async void ChangeMap(int mapindex)
        {
            await webView.EnsureCoreWebView2Async();
            webView.CoreWebView2.Navigate($"{tarkovmarket}{Map[mapindex]}");
        }

        #region 스크린샷 감시
        private async void StartFileMonitoring()
        {
            fileMonitorTokenSource = new CancellationTokenSource();
            var token = fileMonitorTokenSource.Token;

            string[] previousFiles = Directory.GetFiles(EFTScreenshot);

            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (IsRun)
                    {
                        string[] currentFiles = Directory.GetFiles(EFTScreenshot);
                        var newFiles = currentFiles.Except(previousFiles).ToArray();

                        foreach (string newFile in newFiles)
                        {
                            if (File.Exists(newFile))
                            {
                                string xyz;
                                Invoke(new Action(() =>
                                {
                                    browser.InputEnable();
                                    xyz = browser.SetLocation(Path.GetFileNameWithoutExtension(newFile));
                                    browser.Marker();

                                    this.Text = "EFTMap   " + xyz;

                                    if (CB_AutoRemove.Checked)
                                    {
                                        File.Delete(newFile);
                                    }
                                }));
                            }
                        }

                        previousFiles = currentFiles;
                    }

                    await Task.Delay(500, token);
                }
            }
            catch (TaskCanceledException)
            {
                // 정상적으로 취소된 경우 무시
            }
            catch (Exception ex)
            {
                MessageBox.Show($"파일 감시 오류: {ex.Message}");
            }
        }
        private void StopFileMonitoring()
        {
            fileMonitorTokenSource?.Cancel();
            fileMonitorTokenSource?.Dispose();
            fileMonitorTokenSource = null;
        }
        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopFileMonitoring();

            Config.Location.Save(Location);
            Config.MapIndex.Save(comboBox1.SelectedIndex);
            Config.AutoSelect.Save(CB_AutoSelect.Checked);
            Config.AutoRemove.Save(CB_AutoRemove.Checked);

            Environment.Exit(0);
        }
    }
}
