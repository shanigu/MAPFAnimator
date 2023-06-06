using System.Diagnostics;
using System.Linq.Expressions;

namespace MAPFAnimator
{
    public partial class MAPFAnimator : Form
    {
        List<List<int>> Map;
        string MapName;
        Bitmap MapImage;

        List<Color> AgentColors;
        List<Tuple<Point, Point>> StartEndLocations;
        List<List<List<Point>>> Plans;
        List<List<Point>> Steps;
        List<Tuple<int, Point, bool>> Observations;
        Dictionary<Point, int> DoorStatus;

        public MAPFAnimator()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult res = dlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                LoadMap(dlg.FileName);
                btnStart.Text = "Start";
            }
        }

        private void LoadMap(string fileName)
        {
            string sLine = "";
            try
            {

                using (StreamReader sr = new StreamReader(fileName))
                {

                    Map = new List<List<int>>(); ;
                    while (sLine == "" && !sr.EndOfStream)
                    {
                        sLine = sr.ReadLine();
                    }
                    MapName = sLine.Trim();
                    lblMapName.Text = MapName;


                    sLine = "";
                    while (sLine == "" && !sr.EndOfStream)
                    {
                        sLine = sr.ReadLine();
                    }
                    int row = 0;
                    while (sLine != "Agents" && sLine != "" && !sr.EndOfStream)
                    {
                        List<int> lRow = new List<int>();
                        sLine = sLine.Trim().Replace(" ", "");
                        int col = 0;
                        for (int i = 0; i < sLine.Length; i++)
                        {
                            char c = sLine[i];
                            if (c == '.')
                                lRow.Add(0);
                            if (c == '@')
                                lRow.Add(1);
                            if (c == 'T')
                                lRow.Add(2);
                            if (c == '?')
                            {
                                //DoorStatus[new Point(row, col)] = 0;//not marking them here because only some of the obstacles apply here
                                lRow.Add(0);
                            }
                            col++;
                        }
                        Map.Add(lRow);
                        row++;
                        sLine = sr.ReadLine();
                    }

                    StartEndLocations = new List<Tuple<Point, Point>>();
                    while (sLine.Trim() != "#Agents" && !sr.EndOfStream)
                        sLine = sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        sLine = sr.ReadLine().Trim();
                        if (sLine == "" || sLine.StartsWith("#"))
                            break;

                        sLine = sLine.Replace(", ", ",");
                        string[] a = sLine.Split(' ');

                        Point pStart = Parse(a[1]);
                        Point pEnd = Parse(a[2]);

                        Map[pStart.X][pStart.Y] = 0;
                        Map[pEnd.X][pEnd.Y] = 0;

                        StartEndLocations.Add(new Tuple<Point, Point>(pStart, pEnd));
                    }

                    DoorStatus = new Dictionary<Point, int>();
                    while (sLine.Trim() != "#Potential Obstacles" && !sr.EndOfStream)
                        sLine = sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        sLine = sr.ReadLine().Trim();
                        if (sLine == "" || sLine.StartsWith("#"))
                            break;

                        sLine = sLine.Replace(", ", ",");
                        string[] a = sLine.Split(' ');

                        Point p = Parse(a[1]);

                        DoorStatus[p] = 0;
                    }


                    AgentColors = new List<Color>();
                    int cColors = StartEndLocations.Count;
                    for (double dHue = 0.0; dHue < 360.0; dHue += (360.0 / cColors))
                    {
                        Color c = ColorFromHSV(dHue, 0.8, 0.8);
                        AgentColors.Add(c);
                    }

                    MapImage = new Bitmap(picMap.Width, picMap.Height);
                    using (Graphics g = Graphics.FromImage(MapImage))
                    {
                        DrawMap(g, true);
                        DrawObstacles(g);

                    }
                    picMap.Image = MapImage;

                    while (sLine.Trim() != "#Plan" && !sr.EndOfStream)
                    {
                        sLine = sr.ReadLine();
                    }

                    Plans = new List<List<List<Point>>>();
                    ReadPlan(sr);

                    Steps = new List<List<Point>>();
                    Observations = new List<Tuple<int, Point, bool>>();
                    while (sLine.Trim() != "#Steps" && !sr.EndOfStream)
                    {
                        sLine = sr.ReadLine();
                    }

                    int cSteps = 0;

                    while (!sr.EndOfStream)
                    {
                        sLine = sr.ReadLine().Trim();
                        if (sLine == "")
                            continue;
                        if (sLine == "#step")
                        {
                            sLine = sr.ReadLine().Trim();
                            List<Point> lStep = ParsePointList(sLine);
                            Steps.Add(lStep);
                            cSteps++;
                        }
                        if (sLine == "#Plan")
                        {
                            ReadPlan(sr);
                        }
                        if (sLine == "#observation")
                        {
                            sLine = sr.ReadLine().Trim();
                            sLine = sLine.Replace(", ", ",");
                            string[] a = sLine.Split(' ');
                            Point p = Parse(a.Last());
                            bool bOpen = sLine.ToLower().Contains("open");
                            Observations.Add(new Tuple<int, Point, bool>(cSteps, p, bOpen));
                            if (bOpen)
                            {
                                Plans.Add(null);
                            }
                        }
                    }
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error loading log. Error in line: " + sLine + "\n" + "Exception info: " + e.ToString());
            }

        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        private List<Point> ParsePointList(string sLine)
        {
            List<Point> lPoints = new List<Point>();

            sLine = sLine.Replace(", ", ",");
            string[] a = sLine.Split(' ');

            for (int i = 1; i < a.Length; i++)
            {
                Point p = Parse(a[i]);
                lPoints.Add(p);
            }
            return lPoints;
        }

        private void ReadPlan(StreamReader sr)
        {
            Plans.Add(new List<List<Point>>());
            while (!sr.EndOfStream)
            {
                string sLine = sr.ReadLine().Trim();
                if (sLine == "" || sLine.StartsWith("#"))
                    break;

                List<Point> lPlan = ParsePointList(sLine);
                Plans.Last().Add(lPlan);
            }
        }

        private Point Parse(string s)
        {
            s = s.Replace("(", "");
            s = s.Replace(")", "");
            string[] a = s.Split(',');
            int x = int.Parse(a[0]);
            int y = int.Parse(a[1]);
            return new Point(x, y);
        }


        int Rows;
        int Columns;
        int CellWidth;
        int CellHeight;

        private void DrawMap(Graphics g, bool bDrawStartPoistions)
        {
            if (Map == null)
                return;
            picMap.BorderStyle = BorderStyle.FixedSingle;

            Rows = Map.Count;
            Columns = Map[0].Count;
            CellWidth = picMap.Width / Columns;
            CellHeight = picMap.Height / Rows;
            if (CellWidth > CellHeight)
                CellWidth = CellHeight;
            else
                CellHeight = CellWidth;


            for (int col = 0; col < Columns; col++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    int iType = Map[row][col];
                    Brush b = null;
                    b = Brushes.White;
                    if (iType == 1)
                        b = Brushes.Black;
                    if (iType == 2)
                        b = Brushes.Green;
                    //if (iType == 3)
                    //   b = Brushes.Red;

                    DrawAt(g, row, col, b, 0);
                }
            }
            for (int i = 0; i < AgentColors.Count; i++)
            {
                Point pStart = StartEndLocations[i].Item1;
                Point pEnd = StartEndLocations[i].Item2;
                Color c = AgentColors[i];
                Brush b = new SolidBrush(c);
                if (bDrawStartPoistions)
                    DrawAt(g, pStart.X, pStart.Y, b, 1);
                DrawAt(g, pEnd.X, pEnd.Y, b, 2);

            }

            
        }

        private void DrawAt(Graphics g, float row, float col, Brush b, int iType)
        {
            int x = (int)(col * CellWidth);
            int y = (int)(row * CellHeight);
            if (iType == 0) //cell
                g.FillRectangle(b, x, y, CellWidth, CellHeight);
            if (iType == 1) //agent
                g.FillEllipse(b, x, y, CellWidth, CellHeight);
            if (iType == 2) //goal
            {
                Font f = new Font(FontFamily.GenericSansSerif, CellHeight / 2);
                g.DrawString("X", f, b, new PointF(x, y));
            }
            if (iType == 3)//plan
            {
                g.FillEllipse(b, x + CellWidth / 2, y + CellHeight / 2, CellWidth / 4, CellHeight / 4);
            }
            if (iType / 10 == 2)//obstacles
            {
                g.FillRectangle(Brushes.Red, x, y, CellWidth, CellHeight);
                if (iType == 21)//open
                {
                    g.FillRectangle(Brushes.White, x + 3, y + 3, CellWidth - 6, CellHeight - 6);
                }
                if (iType == 22)//open
                {
                    g.FillRectangle(Brushes.Black, x + 3, y + 3, CellWidth - 6, CellHeight - 6);
                }
            }
        }

        private void DrawAt(Graphics g, PointF p, Brush b, int iType)
        {
            DrawAt(g, p.X, p.Y, b, iType);
        }

        private void DrawAt(Graphics g, Point p, Brush b, int iType)
        {
            DrawAt(g, p.X, p.Y, b, iType);
        }

        private System.Windows.Forms.Timer StepTimer;
        private int Step;
        private int NextObservation;

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (Map == null)
                return;

            if (btnStart.Text == "Stop")
            {
                StepTimer.Stop();
                btnStart.Text = "Continue";
                return;
            }

            if (btnStart.Text == "Continue")
            {
                StepTimer.Start();
                btnStart.Text = "Stop";
                return;
            }

            Step = 0;
            NextObservation = 0;
            LastStep = DateTime.Now;

            using (Graphics g = Graphics.FromImage(MapImage))
            {
                DrawMap(g, false);
                DrawObstacles(g);
            }
            CurrentPlan = Plans[0];
            CurrentPlanStep = 0;
            StepTimer = new System.Windows.Forms.Timer();

            StepTimer.Interval = 30;
            StepTimer.Tick += new EventHandler(ShowStep);

            btnStart.Text = "Stop";

            StepTimer.Start();
        }

        private void DrawPlan(Graphics g, List<List<Point>> lPlan, int iStart)
        {

            for (int i = iStart; i < lPlan.Count; i++)
            {
                Color c = AgentColors[i];
                Brush b = new SolidBrush(c);
                foreach (Point p in lPlan[i])
                {
                    DrawAt(g, p.X, p.Y, b, 3);
                }
            }

        }

        private void DrawObstacles(Graphics g)
        {

            foreach (Point p in DoorStatus.Keys)
            {
                DrawAt(g, p, null, 20 + DoorStatus[p]);
            }

        }


        private List<List<Point>> CurrentPlan;
        private int CurrentPlanStep;

        private void ShowStep(object? sender, EventArgs e)
        {

            DateTime dtNow = DateTime.Now;
            int interval = (tbSpeed.Maximum - tbSpeed.Value) * 100 + 100;

            double dTimePortion = (dtNow - LastStep).TotalMilliseconds / interval;

            using (Graphics g = Graphics.FromImage(MapImage))
            {
                if (dTimePortion >= 1)
                {
                    AdvanceStep(g);
                    dTimePortion = 0;
                }
                
                DrawMovement(g, dTimePortion);
            }

            picMap.Refresh();
        }


        private void AdvanceStep(Graphics g)
        {

            lblStep.Text = Step + "/" + Steps.Count;


            while (NextObservation < Observations.Count && Step == Observations[NextObservation].Item1)
            {
                List<List<Point>> lPlan = Plans[NextObservation + 1];
                if (lPlan != null)
                {
                    CurrentPlan = lPlan;
                    CurrentPlanStep = 0;
                }
                Point p = Observations[NextObservation].Item2;
                if (Observations[NextObservation].Item3)
                    DoorStatus[p] = 1;
                else
                    DoorStatus[p] = 2;

                NextObservation++;

                if (chkStop.Checked)
                {
                    StepTimer.Stop();
                    btnStart.Text = "Continue";
                }
                DrawMap(g, false);
                DrawPlan(g, CurrentPlan, CurrentPlanStep);
            }

            DrawObstacles(g);



            Step++;
            CurrentPlanStep++;
            LastStep = DateTime.Now;

            if (Step == Steps.Count)
            {
                StepTimer.Stop();
                btnStart.Text = "Start";
            }
        }


        private void MAPFAnimator_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
        }

        private DateTime LastStep;
        List<PointF> LastPositions;

        

        private void DrawMovement(Graphics g, double dTimePortion)
        {
            if(Step == Steps.Count)
            {
                return;
            }
            List<Point> lPreviousPositions = Steps[Step];
            List<Point> lNextPositions = null;
            if (Step < Steps.Count - 1)
                lNextPositions = Steps[Step + 1];
            else
                lNextPositions = lPreviousPositions;
            //Debug.WriteLine(Step + ": " + dTimePortion);

                //dTimePortion = 0.5;

                //g.FillRectangle(Brushes.Orange, 0, 0, 1000, 1000);

            if (LastPositions != null)
            {
                foreach (PointF p in LastPositions)
                {
                    DrawAt(g, p, Brushes.White, 1);
                }
            }
            List<PointF> lPositions = new List<PointF>();
            for (int i = 0; i < lPreviousPositions.Count; i++)
            {
                Point pPrevious = lPreviousPositions[i];
                Point pNext = lNextPositions[i];
                double dX = pPrevious.X + (pNext.X - pPrevious.X) * dTimePortion;
                double dY = pPrevious.Y + (pNext.Y - pPrevious.Y) * dTimePortion;
                PointF p = new PointF((float)dX, (float)dY);

                //if (i == 0)
                //    Debug.WriteLine(p + ", " + dTimePortion);

                Color c = AgentColors[i];
                Brush b = new SolidBrush(c);
                DrawAt(g, p, b, 1);
                lPositions.Add(p);
            }
            LastPositions = lPositions;
        }
        

        private void picMap_Paint(object sender, PaintEventArgs e)
        {

            
        }
    }
}