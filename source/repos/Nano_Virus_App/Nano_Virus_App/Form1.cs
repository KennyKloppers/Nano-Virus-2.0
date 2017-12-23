using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Nano_Virus_App
{
    public partial class Form1 : Form
    {
        //Globals
        List<Cells> CellsList = new List<Cells>(100);
        List<Cells> TumorList = new List<Cells>(100);
        List<int> TerminateList = new List<int>(100);
        List<int> ConvertedList = new List<int>(100);

        Random random = new Random();

        int nanocurrentCell = 0;
        int nanoX = 0;
        int nanoY = 0;
        int nanoZ = 0;
        string nanoProperty = "";
        int Terminated = 0;

        int Closest = 0;
        int ClosestCount = 0;
        string ClosestProperty = "";
        int PreviousClosestCount = 0;

        int CycleCount = 0;
        int SpreadTumorCount = 4;

        string FileName = "";
        string Date = "";

        public Form1()
        {
            InitializeComponent();

            if (!Directory.Exists(@"C:\NanoVirus\"))
            {
                Directory.CreateDirectory(@"C:\NanoVirus\");
            }
        }

        public void Clear()
        {
            CellsList.Clear();
            TumorList.Clear();
            TerminateList.Clear();

            nanocurrentCell = 0;
            nanoX = 0;
            nanoY = 0;
            nanoZ = 0;
            nanoProperty = "";

            Closest = 0;
            ClosestCount = 0;
            ClosestProperty = "";
            PreviousClosestCount = 0;

            SpreadTumorCount = 5;
            CycleCount = 0;

            FileName = "";
            Date = "";
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            btnStart.BackColor = Color.Red;
            Cursor.Current = Cursors.WaitCursor;

            Clear();

            Date = DateTime.Now.Year.ToString() + " - " + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + " " + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString();
            FileName = "Nano Virus Log(" + Date + ")";

            GetCells();

            while (CellsList.Exists(x => x.Prorerty == "Tumorous" && x.Terminated == 0 ))   
            {
                Nextcycle();
            }

            MainWriter("", FileName);
            MainWriter("Teminated Cells: (" + TumorList.Count.ToString() + ")", FileName);

            //foreach (var C in TumorList)
            //{
            //    MainWriter(C.Count + " - (" + C.Prorerty.ToString() + "), ", FileName);
            //}

            btnStart.BackColor = Color.Gainsboro;
            Cursor.Current = Cursors.Default;

            MessageBox.Show("Results File 'Nano Virus Log(" + Date + ").ini' can be found in " + @"C:\NanoVirus\");
        }

        private void GetCells() //Generate 100 Random Cells - 5% Tumorous - 25% White - 70% Red
        {
            int TCellCount = 0;
            int WCellCount = 0;
            int RCellCount = 0;

            string Property = "";

            int maxCells = 100;
            int percentTumorous = maxCells * 5 / 100;
            int percentWhite = maxCells * 25 / 100;
            int percentRed = maxCells * 70 / 100;

            MainWriter("Generating 100 Cells, Each With X-Y-Z Coordinates.", FileName);
            MainWriter("", FileName);
            MainWriter("Percentage Of Tumorous Cells = " + percentTumorous.ToString() + "%", FileName);
            MainWriter("Percentage Of White Blood Cells = " + percentWhite.ToString() + "%", FileName);
            MainWriter("Percentage Of Red Blood Cells = " + percentRed.ToString() + "%", FileName);
            MainWriter("", FileName);

            for (int i = 0; i < 100; i++)
            {
                Cells cell = new Cells();

                int x = random.Next(1, 5001); // creates a random x coordinate between 1 and 5000
                int y = random.Next(1, 5001); // creates a random y coordinate between 1 and 5000
                int z = random.Next(1, 5001); // creates a random z coordinate between 1 and 5000

                int cellProperty = random.Next(1, 4); // Creates a Random Cell Property Between 1 and 3

                if (cellProperty == 1 && TCellCount < percentTumorous) // 1 = Tumorous Cell
                {
                    Property = "Tumorous";
                    TCellCount++;

                    cell.Count = i;
                    cell.X = x;
                    cell.Y = y;
                    cell.Z = z;
                    cell.Prorerty = Property;
                    cell.Terminated = 0;

                    CellsList.Add(cell);
                }
                else if (cellProperty == 2 && WCellCount < percentWhite) // 2 = White Cell
                {
                    Property = "White";
                    WCellCount++;

                    cell.Count = i;
                    cell.X = x;
                    cell.Y = y;
                    cell.Z = z;
                    cell.Prorerty = Property;
                    cell.Terminated = 0;

                    CellsList.Add(cell);
                }
                else if (cellProperty == 3 && RCellCount < percentRed) // 3 = Red Cell
                {
                    Property = "Red";
                    RCellCount++;

                    cell.Count = i;
                    cell.X = x;
                    cell.Y = y;
                    cell.Z = z;
                    cell.Prorerty = Property;
                    cell.Terminated = 0;

                    CellsList.Add(cell);
                }
                else
                {
                    i = i - 1;
                }
            }

            bool nanoStart = false;

            while (nanoStart == false)
            {
                int nano = GetNano();
                string startCellProperty = CellsList[nano].Prorerty;

                if (startCellProperty == "Red")
                {
                    nanoStart = true;

                    nanocurrentCell = nano;
                    nanoX = CellsList[nano].X;
                    nanoY = CellsList[nano].Y;
                    nanoZ = CellsList[nano].Z;
                    nanoProperty = CellsList[nano].Prorerty;

                    MainWriter("Nano Virus Will Start On Cell " + nanocurrentCell.ToString() + " With The Following Coordinates: ", FileName);
                    MainWriter("X: " + nanoX.ToString() + " - Y: " + nanoY.ToString() + " - Z: " + nanoZ.ToString() + " - Cell Type: " + nanoProperty.ToString(), FileName);
                }
            }
        }

        private int GetNano()
        {
            int nano = random.Next(0, 100);

            return nano;
        }

        private int GetDistance(int x1, int y1, int z1, int x2, int y2, int z2) //Method with distance calculation
        {
            int result = Convert.ToInt32(Math.Sqrt(Math.Pow(Math.Abs(x1 - x2), 2) + Math.Pow(Math.Abs(y1 - y2), 2) + Math.Pow(Math.Abs(z1 - z2), 2)));

            return result;
        } 

        private void Nextcycle()
        {
            foreach (var c in CellsList)
            {
                if (c.Terminated == 0)
                {
                    int distance = GetDistance(c.X, c.Y, c.Z, nanoX, nanoY, nanoZ);

                    c.Distance = distance;
                }
            }

            foreach (var c in CellsList)
            {
                if (c.Prorerty == "Tumorous" && c.Terminated == 0 && c.Distance < 5000)
                {
                    if (c.Distance > 0)
                    {
                        Closest = c.Distance;
                        ClosestCount = c.Count;
                        ClosestProperty = c.Prorerty;

                        if (Closest < 5000)
                        {
                            nanocurrentCell = c.Count;
                            nanoX = c.X;
                            nanoY = c.Y;
                            nanoZ = c.Z;
                            nanoProperty = c.Prorerty;
                            PreviousClosestCount = c.Count;
                            Terminated = c.Terminated;

                            CycleCount++;

                            MainWriter("------------------------------------------", FileName);
                            MainWriter("Cycle: " + CycleCount.ToString(), FileName);
                            MainWriter("------------------------------------------", FileName);
                            MainWriter("Nano Virus Moved to " + nanocurrentCell.ToString() + " (" + nanoProperty.ToString() + ").", FileName);

                            if (CycleCount == SpreadTumorCount)
                            {
                                SpreadTumorCount = SpreadTumorCount + 5;

                                MainWriter("WARNING : Tumorous Cells Have Spread On Cycle Count No " + CycleCount.ToString(), FileName);

                                SpreadTumorousCells();
                            }

                            break;
                        }
                        else
                        {
                            foreach (var C in CellsList)
                            {
                                if ((C.Prorerty == "Red" || C.Prorerty == "White") && C.Terminated == 0 && C.Distance < 5000)
                                {
                                    if (C.Distance > 0)
                                    {
                                        Closest = C.Distance;
                                        ClosestCount = C.Count;
                                        ClosestProperty = C.Prorerty;

                                        nanocurrentCell = C.Count;
                                        nanoX = C.X;
                                        nanoY = C.Y;
                                        nanoZ = C.Z;
                                        nanoProperty = C.Prorerty;
                                        PreviousClosestCount = C.Count;
                                        Terminated = C.Terminated;

                                        CycleCount++;

                                        MainWriter("------------------------------------------", FileName);
                                        MainWriter("Cycle: " + CycleCount.ToString(), FileName);
                                        MainWriter("------------------------------------------", FileName);
                                        MainWriter("Nano Virus Moved To " + nanocurrentCell.ToString() + " (" + nanoProperty.ToString() + ").", FileName);

                                        if (CycleCount == SpreadTumorCount)
                                        {
                                            SpreadTumorCount = SpreadTumorCount + 5;

                                            MainWriter("WARNING : Tumorous Cells Have Spread On Cycle Count No " + CycleCount.ToString(), FileName);

                                            SpreadTumorousCells();
                                        }
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }

            if (nanoProperty == "Tumorous" && Terminated == 0)
            {
                nanocurrentCell = CellsList[ClosestCount].Count;
                nanoX = CellsList[ClosestCount].X;
                nanoY = CellsList[ClosestCount].Y;
                nanoZ = CellsList[ClosestCount].Z;
                nanoProperty = CellsList[ClosestCount].Prorerty;
                CellsList[ClosestCount].Terminated = 1;

                Cells C = new Cells
                {
                    Count = nanocurrentCell,
                    Distance = 0,
                    Prorerty = nanoProperty,
                    Terminated = 1,
                    X = nanoX,
                    Y = nanoY,
                    Z = nanoZ
                };

                TumorList.Add(C);

                CycleCount++;

                MainWriter("------------------------------------------", FileName);
                MainWriter("Cycle: " + CycleCount.ToString(), FileName);
                MainWriter("------------------------------------------", FileName);
                MainWriter("Nano Virus Terminated Cell : " + nanocurrentCell.ToString() + " (" + nanoProperty.ToString() + ").", FileName);

                if (CycleCount == SpreadTumorCount)
                {
                    SpreadTumorCount = SpreadTumorCount + 5;

                    MainWriter("WARNING : Tumorous Cells Have Spread On Cycle Count No " + CycleCount.ToString(), FileName);

                    SpreadTumorousCells();
                }
            }
            else
            {    
                CycleCount++;

                MainWriter("------------------------------------------", FileName);
                MainWriter("Cycle: " + CycleCount.ToString(), FileName);
                MainWriter("------------------------------------------", FileName);
                MainWriter("Nano Did Notning To Current Cell : " + nanocurrentCell.ToString() + " (" + nanoProperty.ToString() + ").", FileName);

                if (CycleCount == SpreadTumorCount)
                {
                    SpreadTumorCount = SpreadTumorCount + 5;

                    MainWriter("WARNING : Tumorous Cells Have Spread On Cycle Count No " + CycleCount.ToString(), FileName);

                    SpreadTumorousCells();
                }
            }
        }

        private void SpreadTumorousCells()
        {
            int TDistance = 0;
            int MinDistance = 0;
            int count = 0;
            int CLCount = 0;
            ConvertedList.Clear();
            
            foreach (var T in CellsList)
            {
                if (ConvertedList.IndexOf(T.Count) != -1)
                {
                    CLCount = T.Count;
                }
                else
                {
                    CLCount = 202;
                }

                if (T.Prorerty == "Tumorous" && T.Terminated == 0 && T.Count != CLCount  && T.Count != 202)
                {
                    if (CellsList.Exists(x => x.Prorerty == "Red" && x.Terminated == 0))
                    {
                        foreach (var c in CellsList)
                        {
                            if (c.Prorerty == "Red")
                            {
                                TDistance = GetDistance(c.X, c.Y, c.Z, T.X, T.Y, T.Z);

                                CellsList[c.Count].Distance = TDistance;
                                ConvertedList.Add(c.Count);
                            }
                        }
                    }
                    else
                    {
                        foreach (var c in CellsList)
                        {
                            if (c.Prorerty == "White")
                            {
                                TDistance = GetDistance(c.X, c.Y, c.Z, T.X, T.Y, T.Z);

                                CellsList[c.Count].Distance = TDistance;
                                ConvertedList.Add(c.Count);
                            }
                        }
                    }

                    MinDistance = CellsList.Min(o => o.Distance);

                    count = CellsList.FindIndex(o => o.Distance == MinDistance);

                    CellsList[count].Distance = TDistance;
                    CellsList[count].Prorerty = "Tumorous";

                    MainWriter("WARNING : Cell " + count.ToString() + " Converted To Tumorous Cell.", FileName);
                }
            }
        }

        public class Cells
        {
            public int Count { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }
            public string Prorerty { get; set; }
            public int Terminated { get; set; }
            public int Distance { get; set; }
        }

        public void MainWriter(string Message, string filename)
        {
            StreamWriter file = new StreamWriter(@"C:\NanoVirus\" + filename + ".ini",true);

            file.WriteLine(Message.ToString());
            file.NewLine.ToString();
            file.Close();
        }
    }
}
