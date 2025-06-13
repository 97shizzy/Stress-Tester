using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StressTester
{
    public partial class Form1 : Form
    {
        private Thread? cpuThread;
        private Thread? ramThread;
        private bool cpuRunning = false;
        private bool ramRunning = false;

        private DateTime cpuStartTime;
        private DateTime ramStartTime;

        private Label cpuTimeLabel;
        private Label ramTimeLabel;

        private Button cpuButton;
        private Button ramButton;

        private System.Windows.Forms.Timer uiTimer;

        public Form1()
        {
            InitializeComponent();

            this.Text = "Stress Tester";
            this.Size = new Size(400, 250);
            this.BackColor = Color.FromArgb(240, 240, 240);

            cpuButton = new Button() { Text = "Start CPU Load", Location = new Point(20, 20), Size = new Size(150, 40) };
            cpuButton.Click += CpuButton_Click;
            cpuButton.BackColor = Color.LightSteelBlue;
            cpuButton.FlatStyle = FlatStyle.Flat;
            this.Controls.Add(cpuButton);

            cpuTimeLabel = new Label() { Text = "CPU Time: 00:00:00.000", Location = new Point(200, 30), AutoSize = true, ForeColor = Color.DarkBlue, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            this.Controls.Add(cpuTimeLabel);

            ramButton = new Button() { Text = "Start RAM Load", Location = new Point(20, 80), Size = new Size(150, 40) };
            ramButton.Click += RamButton_Click;
            ramButton.BackColor = Color.LightCoral;
            ramButton.FlatStyle = FlatStyle.Flat;
            this.Controls.Add(ramButton);

            ramTimeLabel = new Label() { Text = "RAM Time: 00:00:00.000", Location = new Point(200, 90), AutoSize = true, ForeColor = Color.DarkRed, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            this.Controls.Add(ramTimeLabel);

            uiTimer = new System.Windows.Forms.Timer();
            uiTimer.Interval = 50; 
            uiTimer.Tick += UiTimer_Tick;
            uiTimer.Start();
        }

        private void CpuButton_Click(object? sender, EventArgs e)
        {
            if (!cpuRunning)
            {
                cpuRunning = true;
                cpuStartTime = DateTime.Now;
                cpuButton.Text = "Stop CPU Load";
                cpuButton.BackColor = Color.LightGreen;

                cpuThread = new Thread(() =>
                {
                    while (cpuRunning)
                    {
                        Parallel.For(0, Environment.ProcessorCount * 2, _ =>
                        {
                            while (cpuRunning)
                            {
                                double x = Math.Pow(new Random().NextDouble(), 3.14);
                            }
                        });
                    }
                });
                cpuThread.IsBackground = true;
                cpuThread.Start();
            }
            else
            {
                cpuRunning = false;
                cpuButton.Text = "Start CPU Load";
                cpuButton.BackColor = Color.LightSteelBlue;
                cpuThread?.Join();
            }
        }

        private void RamButton_Click(object? sender, EventArgs e)
        {
            if (!ramRunning)
            {
                ramRunning = true;
                ramStartTime = DateTime.Now;
                ramButton.Text = "Stop RAM Load";
                ramButton.BackColor = Color.LightGreen;

                ramThread = new Thread(() =>
                {
                    var list = new List<byte[]>();
                    try
                    {
                        while (ramRunning)
                        {
                            var block = new byte[1000 * 1024 * 1024]; // 1000 MB
                            for (int i = 0; i < block.Length; i += 4096)
                            {
                                block[i] = 1; 
                            }
                            list.Add(block);
                            Thread.Sleep(100);
                        }
                    }
                    catch (OutOfMemoryException)
                    {
                        
                    }
                });
                ramThread.IsBackground = true;
                ramThread.Start();
            }
            else
            {
                ramRunning = false;
                ramButton.Text = "Start RAM Load";
                ramButton.BackColor = Color.LightCoral;
                ramThread?.Join();
            }
        }

        private void UiTimer_Tick(object? sender, EventArgs e)
        {
            if (cpuRunning)
            {
                TimeSpan elapsed = DateTime.Now - cpuStartTime;
                cpuTimeLabel.Text = $"CPU Time: {elapsed:hh\\:mm\\:ss\\.fff}";
            }
            else
            {
                cpuTimeLabel.Text = "CPU Time: 00:00:00.000";
            }

            if (ramRunning)
            {
                TimeSpan elapsed = DateTime.Now - ramStartTime;
                ramTimeLabel.Text = $"RAM Time: {elapsed:hh\\:mm\\:ss\\.fff}";
            }
            else
            {
                ramTimeLabel.Text = "RAM Time: 00:00:00.000";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            cpuRunning = false;
            ramRunning = false;
            cpuThread?.Join();
            ramThread?.Join();
            base.OnFormClosing(e);
        }
    }
}
