using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;
using BeatDetectorCSharp;

namespace gaem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        PictureBox topEU = new PictureBox();
        ComboBox songs = new ComboBox();
        Button loadSongs = new Button();
        ComboBox difficulty = new ComboBox();

        private void SetForm()
        {
            //set form properties
            ForeColor = Color.White;
            BackColor = Color.AliceBlue;
            Size = new Size(300,400);
            Text = "Beta Version 2.1.4";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;

            //design       
            topEU.Location = new Point(0,0);
            topEU.Size = new Size(286,386);
            topEU.Image = Properties.Resources.topEU;    
            Controls.Add(topEU);       

            //songs combobox
            songs.Location = new Point(10,10);
            songs.Size = new Size(210,30);
            songs.DropDownStyle = ComboBoxStyle.DropDownList;
            songs.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            songs.FlatStyle = FlatStyle.Flat;
            songs.BackColor = Color.DarkSlateBlue;
            songs.ForeColor = Color.White;
            Controls.Add(songs);
            songs.SelectedIndexChanged += new EventHandler(songs_SelectedIndexChanged);
            songs.BringToFront();
 
            //loadsongs button
            loadSongs.Location = new Point(10,40);
            loadSongs.Size = new Size(80, 25);
            loadSongs.Text = "Load Music";
            loadSongs.FlatStyle = FlatStyle.Flat;
            loadSongs.Font = new Font("Segoe UI", 8, FontStyle.Regular);
            loadSongs.BackColor = Color.DarkSlateBlue;
            loadSongs.ForeColor = Color.White;
            Controls.Add(loadSongs);
            loadSongs.Click += new EventHandler(loadSongs_Click);
            loadSongs.BringToFront();

            //difficulty select
            difficulty.Location = new Point(10,70);
            difficulty.Size = new Size(80,30);
            difficulty.DropDownStyle = ComboBoxStyle.DropDownList;
            difficulty.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            difficulty.FlatStyle = FlatStyle.Flat;
            difficulty.BackColor = Color.DarkSlateBlue;
            difficulty.ForeColor = Color.White;
            difficulty.Items.Add("Beginner");
            difficulty.Items.Add("Intermediate");
            difficulty.Items.Add("Advanced");
            difficulty.Items.Add("Expert");
            Controls.Add(difficulty);
            difficulty.SelectedIndex = 0;
            difficulty.BringToFront();
        }

        int currentSongIndex;
        string[] songPaths = new string[100];
        private void songs_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentSongIndex = songs.SelectedIndex;
        }

        OpenFileDialog ofd = new OpenFileDialog();

        private void loadSongs_Click(object sender, EventArgs e)
        {
            ofd.Title = "Load Song";
            ofd.Filter = "MP3 Files|*.mp3|WAV Files|*.wav";
           
            DialogResult res = new DialogResult();
            res = ofd.ShowDialog();
            if (res == DialogResult.OK)
            {
                string myPath = ofd.FileName;
                string songName = Path.GetFileNameWithoutExtension(myPath);

                if (songs.Text == "")
                {
                    songPaths[0] = myPath;
                    songs.Items.Add(songName);
                    songs.SelectedItem = songName;
                }
                else
                {
                    songPaths[songs.Items.Count] = myPath;
                    songs.Items.Add(songName);
                    songs.SelectedItem = songName;
                }
                       
            }
        }

        PictureBox pic = new PictureBox();
        private void SetHitField()
        {
            //create hit field
            pic.Size = new Size(300,20);
            pic.Location = new Point(0,260);
            pic.SizeMode = PictureBoxSizeMode.StretchImage;
            pic.Image = Properties.Resources.hoho;     
            Controls.Add(pic);
            pic.BringToFront();
        }

        Button start = new Button();
        private void StartButtonInitialize()
        {
            //create start button
            start.Text = "B E G I N";
            start.Font = new Font("Segoe UI", 7, FontStyle.Bold);
            start.ForeColor = Color.White;
            start.BackColor = Color.SlateBlue;
            start.Location = new Point(10,315);
            start.Size = new Size(210, 40);
            start.Click += Start_Click;
            Controls.Add(start);
            start.BringToFront();
        }

        BeatDetector detector;
        int delay;
        int perfect = 258;
        myProgBar hitBar = new myProgBar();

        private void SetHitBar()
        {
            hitBar.Dock = DockStyle.Top;
            hitBar.BackColor = Color.AliceBlue;
            hitBar.ForeColor = Color.DarkBlue;
            hitBar.Maximum = 2000;
            hitBar.Minimum = 0;
            hitBar.Value = 2000;
            hitBar.Style = ProgressBarStyle.Continuous;
            hitBar.Size = new Size(100,10);
            Controls.Add(hitBar);
            hitBar.BringToFront();
        }

        int gamespeed = 1;

        private void SetDifficulty(string diff)
        {
            if (diff == "Beginner")
            {
                gamespeed = 1;
            }
            else if (diff == "Intermediate")
            {
                gamespeed = 2;
            }
            else if (diff == "Advanced")
            {
                gamespeed = 3;
            }
            else if (diff == "Expert")
            {
                gamespeed = 4;
            }
        }

        private void Start_Click(object sender, EventArgs e)
        {
            if (songs.Text == "")
            {
                
            }
            else
            {
                //set difficulty
                SetDifficulty(difficulty.Text);
                delay = (280 / gamespeed);
                
 
                //create a new instance of beat detector and load FMOD system
                detector = BeatDetector.Instance();
                detector.loadSystem();

                //load song in array
                detector.LoadSong(1024, songPaths[currentSongIndex]);

                //delay spawn to time beats
                detector.loadSongToDelay(delay);

                //start the song
                detector.setStarted(true);

                //start gravity timer
                gravity.Enabled = true;
                gravity.Start();           

                //hide main menu UI
                start.Visible = false;
                songs.Visible = false;
                loadSongs.Visible = false;
                difficulty.Visible = false;
                topEU.SendToBack();

                

                //show in-game UI
                DisplayScore();
                SetHitField();
                SetHitBar();
                highScore.Visible = true;
                scoreDisplay.Visible = true;              
                NowPlaying();
                np.Visible = true;
                np.Text = "Now Playing:\n" + songs.Text;
                Focus();
                           
            }


        }

        Random rnd = new Random();

        //base class for buttons
        public class myButtons
        {
            public Color getBtnColor()
            {
                return Color.Black;
            }

            public FlatStyle getStyle()
            {
                return FlatStyle.Flat;
            }

            public Font getFont()
            {
                return new Font("Segoe UI",5,FontStyle.Regular);
            }

            public Size getSize()
            {
                return new Size(68,25);
            }

            
            public Point randLoc(int x)
            {              
                    int[] locs = { 0, 71, 142, 213 };
                    return new Point(locs[x], -22);         
            }        
        }

        //hitboxes
        int deleteLocation = 362;
        int minHit = 240;
        int maxHit = 276;
          
        //form load to initialize controls
        private void Form1_Load(object sender, EventArgs e)
        {
            SetForm();
            StartButtonInitialize();        
        }
      
        int penalty;
   
        private TimeStamp localLastbeat;
        
        //move ALL tiles simultaneously
        private void gravity_Tick(object sender, EventArgs e)
        {
          
            if (hitBar.Value == 0)
            {
                //initialize gameover
                GameOver();
            }
            else
            {
                //detect beats
                detector.update();
          
                if (localLastbeat != detector.getLastBeat())
                {
                    
                    //spawn here
                    Button btn = new Button();
                    myButtons butt = new myButtons();
                    btn.BackColor = butt.getBtnColor();
                    btn.FlatStyle = butt.getStyle();
                    btn.Size = butt.getSize();
                    btn.Font = butt.getFont();

                    btn.Location = butt.randLoc((rnd.Next(0,4)));

                    if (btn.Location == new Point(0, -22))
                    {
                        btn.Text = "D";
                    }
                    else if (btn.Location == new Point(71, -22))
                    {
                        btn.Text = "F";
                    }
                    else if (btn.Location == new Point(142, -22))
                    {
                        btn.Text = "J";
                    }
                    else if (btn.Location == new Point(213, -22))
                    {
                        btn.Text = "K";
                    }

                    Controls.Add(btn);
                    btn.BringToFront();

                    //update last beat
                    localLastbeat = detector.getLastBeat();
                }
              
                //iterate through buttons
                
                foreach (Button item in Controls.OfType<Button>())
                {
                       if (item != start && item != loadSongs)
                       {
                           //check if item is beyond deleteLocation
                           if (item.Top > deleteLocation)
                           {
                               //remove form controls
                               Controls.Remove(item);
                               //dispose button in array
                               item.Dispose();
                           }
                           else
                           {
                               //move tiles per tick
                               item.Top += gamespeed;

                           if (item.Top > maxHit && item.BackColor == Color.Black)
                           {
                               item.BackColor = Color.Red;
                               item.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                               item.Text = "Missed!";
                               penalty = gamespeed * 10;
                               DeductBar(penalty);
                               //deduct hitbar value
                           }
                        }
                      }

                    }
                }

            //display score    
            scoreDisplay.Text = "Score: " + Convert.ToString(score);
            //get high Score
            SetHighScore();



            if (detector.isPlaying() == false)
            {
                GameOver();
            }

        }

        private void DeductBar(int decrease)
        {
            if ((hitBar.Value - decrease) < 0)
            {
                hitBar.Value = 0;
            }
            else
            {
                hitBar.Value -= decrease;
            }
        }

        private void IncrementBar(int increase)
        {
            if ((hitBar.Value + increase) > 2000)
            {
                hitBar.Value = 2000;
            }
            else
            {
                hitBar.Value += increase;
            }
        }
      
        //initial score
        int score = 0;
        int high = gaem.Properties.Settings.Default.hScore;

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            
            foreach (Button tile in Controls.OfType<Button>())
            {
                //determine if tile is eligilble for hit
                if (tile.Top > minHit && tile.Top < maxHit)
                {
                    //if tile has been hit, do nothing
                    if (tile.BackColor == Color.Blue || tile.BackColor == Color.Red)
                    {

                    }
                    else
                    {
                        string kPress = Convert.ToString(e.KeyCode);
                        if (kPress == tile.Text.ToUpper())
                        {
                            //correct press
                            tile.BackColor = Color.Blue;
                            score += gamespeed * 100;
                            IncrementBar(10);

                            if (tile.Top > perfect - 2 && tile.Top < perfect + 2)
                            {
                                tile.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                                tile.Text = "Perfect!";
                                score += gamespeed * 50;
                                IncrementBar(20);
                            }
                                                    
                        }
                        else if(kPress != tile.Text.ToUpper())
                        {
                            //wrong press
                            tile.BackColor = Color.Red;
                            tile.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                            tile.Text = "Missed";
                            penalty = gamespeed * 10;
                            DeductBar(penalty);                   
                        }
                    }
                    
                }
               
            }
        }

        //gameover sequence and destructor
        public void GameOver()
        {
            //stop timer
            gravity.Stop();
            gravity.Enabled = false;

            //reinitialize score
            score = 0;

            //remove all buttons
            for (int ix = Controls.Count - 1; ix >= 0; ix--)
            {
                if (Controls[ix] is Button && Controls[ix] != start && Controls[ix] != loadSongs)
                {
                    Controls[ix].Dispose();
                }
            }

            //get system and then release it to be able to load a new song
            FMOD.System sys = new FMOD.System();
            sys = detector.getSystem();
            sys.release();
            sys.close();

            //messagebox showing high score
            MessageBox.Show("Game Over" + "\nHigh Score: " + Convert.ToString(high), "Euphoria", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            //show hidden UI
            start.Visible = true;
            songs.Visible = true;
            loadSongs.Visible = true;
            difficulty.Visible = true;

            //hide in-game UI
            highScore.Visible = false;
            scoreDisplay.Visible = false;
            np.Visible = false;

            //remove hitfield
            Controls.Remove(pic);
            Controls.Remove(hitBar);
            
        }

        //score label
        Label scoreDisplay = new Label();
        Label highScore = new Label();

        public void DisplayScore()
        {                     
            scoreDisplay.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            scoreDisplay.Location = new Point(10,20);
            scoreDisplay.BackColor = Color.Transparent;
            scoreDisplay.ForeColor = Color.DarkBlue;
            scoreDisplay.Size = new Size(80,20);
            Controls.Add(scoreDisplay);
            scoreDisplay.Parent = topEU;       

            //high score
            highScore.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            highScore.Location = new Point(10,33);
            highScore.BackColor = Color.Transparent;
            highScore.ForeColor = Color.BlueViolet;
            highScore.Size = new Size(80, 20);
            Controls.Add(highScore);
            highScore.Parent = topEU;

            scoreDisplay.BringToFront();
            highScore.BringToFront();
        }

        //determine high score
        public void SetHighScore()
        {
            if (score > high)
            {
                high = score;
                highScore.Text = "Best: " + Convert.ToString(high);
            }
        }

        //save high score on app.config
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            gaem.Properties.Settings.Default.hScore = high;
            gaem.Properties.Settings.Default.Save();
        }

        //now playing. insert timestamp and title
        Label np = new Label();
        TimeStamp ts = new TimeStamp();
        
        private void NowPlaying()
        {
            np.BackColor = Color.Transparent;
            np.ForeColor = Color.Indigo;
            np.Location = new Point(10,331);
            np.Size = new Size(265,25);
            np.Font = new Font("Segoe UI",7,FontStyle.Bold);
            Controls.Add(np);
            np.BringToFront();
            np.Parent = topEU;     
        }
        
    }
}
