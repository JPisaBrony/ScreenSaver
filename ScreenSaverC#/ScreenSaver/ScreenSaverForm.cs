using AxWMPLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ScreenSaver
{
    public partial class ScreenSaverForm : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);
        private Point mouseLocation;
        private Random rand = new Random();
        private bool previewMode = false;
        private string folder = @"C:\Users\JoshP\Downloads\Videos";
        private string[] files;

        public ScreenSaverForm()
        {
            InitializeComponent();
        }

        public ScreenSaverForm(IntPtr PreviewWndHandle)
        {
            InitializeComponent();

            // Set the preview window as the parent of this window
            SetParent(this.Handle, PreviewWndHandle);

            // Make this a child window so it will close when the parent dialog closes
            // GWL_STYLE = -16, WS_CHILD = 0x40000000
            SetWindowLong(this.Handle, -16, new IntPtr(GetWindowLong(this.Handle, -16) | 0x40000000));

            // Place our window inside the parent
            Rectangle ParentRect;
            GetClientRect(PreviewWndHandle, out ParentRect);
            Size = ParentRect.Size;
            Location = new Point(0, 0);

            previewMode = true;
        }

        public ScreenSaverForm(Rectangle Bounds)
        {
            InitializeComponent();
            this.Bounds = Bounds;
        }

        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {
            Cursor.Hide();
            TopMost = true;

            axWindowsMediaPlayer1.settings.setMode("loop", true);
            axWindowsMediaPlayer1.PlayStateChange += player_PlayStateChange;

            //axWindowsMediaPlayer1.ClickEvent += playerClicked;

            files = Directory.GetFiles(folder);
            randomVideoFromFolder();
        }

        private void randomVideoFromFolder()
        {
            var url = files[rand.Next(files.Length)];
            BeginInvoke(new Action(() => {
                axWindowsMediaPlayer1.URL = url;
            }));
        }

        private void player_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            switch (e.newState)
            {
                // paused
                case 2:
                    Application.Exit();
                    break;
                // playing
                //case 3:
                //    axWindowsMediaPlayer1.fullScreen = true;
                //    axWindowsMediaPlayer1.stretchToFit = true;
                //    break;
                // media ended
                case 8:
                    randomVideoFromFolder();
                    break;
                default:
                    break;
            }
        }

        private void playerClicked(Object sender, _WMPOCXEvents_ClickEvent e)
        {
            Application.Exit();
        }

        private void ScreenSaverForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseLocation.IsEmpty)
            {
                // Terminate if mouse is moved a significant distance
                if (Math.Abs(mouseLocation.X - e.X) > 5 ||
                    Math.Abs(mouseLocation.Y - e.Y) > 5)
                    Application.Exit();
            }

            // Update current mouse location
            mouseLocation = e.Location;
        }

        private void ScreenSaverForm_MouseClick(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }

        private void ScreenSaverForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!previewMode)
                Application.Exit();
        }

    }
}
