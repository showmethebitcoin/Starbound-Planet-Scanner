/*
Copyright (c) 2014 Nicholas Tantillo

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using Gma.UserActivityMonitor;
using System.IO;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml;


namespace Starbound_Planet_Tagger
{
    public partial class Form1 : Form
    {
        SBSymbolTable Font = null;
        SBSymbolTable WindowMarker = null;
        SBSymbolTable FontRed = null;

        Viewport PlanetPic = null;
        Viewport PlanetName = null;
        Viewport PlanetSmallName = null;
        Viewport PlanetInfo = null;
        Viewport PlanetLevel = null;
        Viewport PlanetCoords = null;

        DataSet XmlData;

        bool IsBusy = false;

        string DBFile = "data\\sbdb.xml";


        public void CheckForPrintScreen(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.PrintScreen))
            {
                if (ProcessImage.IsBusy)
                    return;

                PlanetPic = null;
                PlanetName = null;
                PlanetSmallName = null;
                PlanetInfo = null;
                PlanetLevel = null;
                PlanetCoords = null;

                pictureBox7.Image = Bitmap.FromFile("scanning.png");

                ProcessImage.RunWorkerAsync();
            }

           
        }

        public Form1()
        {
            InitializeComponent();
            LoadSymbols.RunWorkerAsync();

            XmlData = ReadSBDB(DBFile);

            dataGridView1.DataSource = XmlData.Tables[0];

           HookManager.KeyDown += CheckForPrintScreen;

           dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
           dataGridView1.UserDeletedRow += dataGridView1_UserDeletedRow;
        }

        private DataSet ReadSBDB(string XmlPath, string schemaPath = "schema.xml")
        {
            var nXmlData = new DataSet();
            nXmlData.ReadXmlSchema(schemaPath);

            if (!Directory.Exists("data"))
            {
                Directory.CreateDirectory("data");
            }


            var DefaultXML = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><NewDataSet>\n<sbplanet>\n<System>Starbound Planet Scanner</System>\n<Name></Name>\n<X>0</X>\n" +
 "<Y>0</Y>\n<Biome></Biome>\n<Threat></Threat>\n<Notes>Enter notes here</Notes><InProgress></InProgress><Done></Done>\n</sbplanet></NewDataSet>";

            if (!File.Exists(XmlPath))
            {
                System.IO.File.WriteAllText(XmlPath, DefaultXML);
            }

            try
            {

                nXmlData.ReadXml(XmlPath, XmlReadMode.InferSchema);
            }
            catch (Exception ex)
            {
                File.Copy(XmlPath, XmlPath + DateTime.Now.Ticks + ".corrupt");
                System.IO.File.WriteAllText(XmlPath, DefaultXML);
                nXmlData.ReadXml(XmlPath, XmlReadMode.InferSchema);
            }

            if (nXmlData.Tables.Count == 0)
            {

                System.IO.File.WriteAllText(XmlPath, DefaultXML);
                nXmlData.ReadXml(XmlPath);
            }

            return nXmlData;
        }

        void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            XmlData.WriteXml(DBFile);
        }

        void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            XmlData.WriteXml(DBFile);
        }

        private void LoadSymbols_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Font == null) {
                Font = new SBSymbolTable(Color.FromArgb(0xff, 0xff, 0xff, 0xff));
            Font.AddDir("~\\symbols");

            FontRed = new SBSymbolTable(Color.FromArgb(0xff, 0xff, 0x00, 0x00));
            FontRed.AddDir("~\\symbols");

            WindowMarker = new SBSymbolTable(Color.FromArgb(0xff, 0x00, 0x00, 0x00));
            WindowMarker.AddDir("~\\windowmarker");

            }
        }

        void button1_Click(object sender, EventArgs e)
        {
            if (ProcessImage.IsBusy)
                return;

            PlanetPic = null;
            PlanetName = null;
            PlanetSmallName = null;
            PlanetInfo = null;
            PlanetLevel = null;
            PlanetCoords = null;

            pictureBox7.Image = Bitmap.FromFile("scanning.png");

            ProcessImage.RunWorkerAsync();

        }

        bool SeekWindow()
        {
            using (Bitmap bmpScreenCapture = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                            Screen.PrimaryScreen.Bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bmpScreenCapture))
                {
                    g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                     Screen.PrimaryScreen.Bounds.Y,
                                     0, 0,
                                     bmpScreenCapture.Size,
                                     CopyPixelOperation.SourceCopy);




                }

                var Prev = new Bitmap(bmpScreenCapture, bmpScreenCapture.Width / 4, bmpScreenCapture.Height / 4);
                  pictureBox1.Image = Prev ;

              

                Viewport V = new Viewport(0, 0, bmpScreenCapture.Width, bmpScreenCapture.Height, bmpScreenCapture);

                for (int ty = 0; ty < bmpScreenCapture.Height; ty++)
                {
                    for (int tx = 0; tx < bmpScreenCapture.Width; tx++)
                    {

                        V.Offset(tx, ty);

                        var PixelMatch = WindowMarker.GetMatch(V);
                        if (PixelMatch != null)
                        {
                            Console.WriteLine("Found Match for {0} at ({1},{2})", PixelMatch.TextConversion, tx, ty);
                            PlanetPic = new Viewport(tx - 15, ty + 40, 357, 291, bmpScreenCapture);
                            PlanetName = new Viewport(tx + 329, ty + 50, 235, 21, bmpScreenCapture);
                            PlanetSmallName = new Viewport(tx + 329, ty + 71, 235, 21, bmpScreenCapture);
                            PlanetInfo = new Viewport(tx + 329, ty + 105, 215, 21, bmpScreenCapture);
                            PlanetLevel = new Viewport(tx + 329, ty + 126, 215, 21, bmpScreenCapture);
                            PlanetCoords = new Viewport(tx + 342, ty + 380, 225, 25, bmpScreenCapture);
                            ty = bmpScreenCapture.Height;
                            tx = bmpScreenCapture.Width; // Break out of double loop
                            ProcessImage.ReportProgress(100);
                        }



                    }
                    ProcessImage.ReportProgress((int)(100 * ty / bmpScreenCapture.Height));
                }

                return PlanetPic != null;
            }
        }

        void ProcessImage_DoWork(object sender, DoWorkEventArgs e)
        {

            if (IsBusy)
                return;

            IsBusy = true;

            var found = SeekWindow();



            if (found)
            {
                pictureBox1.Image = PlanetPic.GetImage();
                pictureBox2.Image = PlanetName.GetImage();
                pictureBox3.Image = PlanetSmallName.GetImage();
                pictureBox4.Image = PlanetInfo.GetImage();
                pictureBox5.Image = PlanetLevel.GetImage();
                pictureBox6.Image = PlanetCoords.GetImage();

                string pSystem = "", Name = "", X = "", Y = "", Biome = "", Threat = "";

                var Running = new List<Task>();

                Running.Add(Task.Run(() =>
                {
                    var PlanetNameText = ReadAndSortCharacters(PlanetName, Font);

                    Console.WriteLine(PlanetNameText);

                    pSystem = FixZero(PlanetNameText);
                }));


                Running.Add(Task.Run(() =>
                {
                    var PlanetSmallNameText = ReadAndSortCharacters(PlanetSmallName, Font);

                    var PlanetChar = PlanetSmallNameText;

                    if (PlanetChar.Length > 0)
                    {
                        PlanetChar = PlanetChar.Substring(PlanetChar.Length - 1);
                    }
                    Console.WriteLine(PlanetChar);

                    Name = FixZero(PlanetChar);

                }));


                Running.Add(Task.Run(() =>
                {
                    var PlanetInfoText = ReadAndSortCharacters(PlanetInfo, Font);
                    Console.WriteLine(PlanetInfoText);
                    Biome = FixZero(PlanetInfoText);
                }));


                Running.Add(Task.Run(() =>
                {
                    var PlanetLevelText = ReadAndSortCharacters(PlanetLevel, Font);
                    Console.WriteLine(PlanetLevelText);
                    Threat = FixZero(PlanetLevelText);
                }));

                Running.Add(Task.Run(() =>
                {
                    var PlanetCoordsText = ReadAndSortCharacters(PlanetCoords, FontRed);
                    Console.WriteLine(PlanetCoordsText);

                    var Coord = FixZero(PlanetCoordsText);

                    if (Coord.Length > 0 && Coord.IndexOf(" ") > 0)
                    {

                        var C = Coord.Split(new char[] { ' ' });
                        X = C[0];
                        Y = C[1];

                    }

                }));

                foreach (var T in Running)
                {
                    T.Wait();
                }

                // Fix 0/O problem

                if (!Directory.Exists("planetpics"))
                    Directory.CreateDirectory("planetpics");

                var Note = "";

                try
                {
                    PlanetPic.GetImage().Save(("planetpics\\" + pSystem + Name + ".png").Replace(" ", ""));
                }
                catch (Exception ex)
                {
                    Note = "System Note: Unable to save planet image";
                }

                XmlData.Tables[0].Rows.Add(pSystem, pSystem + " " + Name, X, Y, Biome.Replace("Biome:", ""), Threat.Replace("Threat Level:", ""), Note);
                dataGridView1.Invoke(new MethodInvoker(() => { IsBusy = false; pictureBox7.Image = null; dataGridView1.Refresh(); XmlData.WriteXml(DBFile); }));

                System.Media.SystemSounds.Exclamation.Play();

            }
            else
            {
              //  pictureBox1.Image = Bitmap.FromFile("notfound.png");
                pictureBox2.Image = Bitmap.FromFile("notfound.png");
                pictureBox3.Image = null;
                pictureBox4.Image = null;
                pictureBox5.Image = null;
                pictureBox6.Image = null;

                IsBusy = false;
                pictureBox7.Image = null;

                System.Media.SystemSounds.Hand.Play();
            }
      



          
        }

        public string FixZero(string input)
        {
            var i = input.IndexOf("0O");
            if (i == -1)
                return input;

            if (i == 0)
                return input.Substring(1);
            char preceeding = input.ToCharArray()[i-1];
            if (preceeding < 48 || preceeding > 57)
                return input.Replace("0O", "O");
            else
                return input.Replace("0O", "0");
        }

        string ReadAndSortCharacters(Viewport VP, SBSymbolTable UseFont)
        {
            List<SBSymbolMatch> Chars = new List<SBSymbolMatch>();

            var PlanetNameTest = new Viewport(0, 0, VP.Width, VP.Height, VP);

            for (int ty = 0; ty < VP.Height; ty++)
            {
                for (int tx = 0; tx < VP.Width; tx++)
                {
                    PlanetNameTest.Offset(tx, ty);

                    var Match = UseFont.GetMatch(PlanetNameTest);
                    if (Match != null) { 
                        Chars.Add(new SBSymbolMatch(tx, ty, Match));
                        

                    }
                }
            }

            Chars.Sort((M1, M2) => M1.x.CompareTo(M2.x));
            StringBuilder SB = new StringBuilder();

            var PrevPos = 100000;

            for (int i = 0; i < Chars.Count; i++)
            {
                if (Chars[i].x - PrevPos > 0)
                {
                    SB.Append(" ");
                }

                SB.Append(Chars[i].MatchObject.TextConversion);
                PrevPos = Chars[i].x + Chars[i].MatchObject.Width;
            }

            return SB.ToString();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void LoadSymbols_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button1.Enabled = true;
            
        }

        private void ProcessImage_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
           
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ProcessImage_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
         //   dataGridView1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Refresh();
        }

        string CurrentPic = "";

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 0)
                return;

            if (dataGridView1.SelectedCells[0].OwningRow.IsNewRow)
                return;

            // Should put regex here with more restrictions
            var OName = dataGridView1.SelectedCells[0].OwningRow.Cells[1].Value.ToString().Replace("\\", "");
            var Name = (("planetpics\\" + OName + ".png").Replace(" ", ""));

            if (!String.IsNullOrEmpty(OName) && !Name.Equals(CurrentPic) &&  File.Exists(Name))
            {
                CurrentPic = Name;
                pictureBox1.Image = Bitmap.FromFile(Name);
                pictureBox2.Image = null;
                pictureBox3.Image = null;
                pictureBox4.Image = null;
                pictureBox5.Image = null;
                pictureBox6.Image = null;

                pictureBox7.Image = null;

            }
            else if (!File.Exists(Name) || String.IsNullOrEmpty(OName))
            {
                pictureBox1.Image = Bitmap.FromFile("Thanks.png");
                CurrentPic = "";
            }
            

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            dataGridView1.Refresh();
            XmlData.WriteXml(DBFile);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ProgramVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name+" v"+System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()+"\n© 2014, Nicholas Tantillo\nAvailable under MIT License";


            }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/showmethebitcoin/Starbound-Planet-Scanner");   
        }

        public string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            if (Directory.Exists(tempDirectory))
                Directory.Delete(tempDirectory, true);

            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

      

        private void exportbutton_Click(object sender, EventArgs e)
        {
            var Rows = dataGridView1.SelectedRows;



            if (Rows.Count == 0 || (Rows.Count == 1 && Rows[0].IsNewRow))
            {
                MessageBox.Show("Select rows by holding the shift or control key and clicking to the left of the first cell of the row you want to add");
                return;
            }

            if (exportRowsDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var FName = exportRowsDialog.FileName;

            BackgroundWorker BGW = new BackgroundWorker();

            exportbutton.Enabled = false;




            BGW.DoWork += (o,args) =>
            {

               

                var Dir = GetTemporaryDirectory();
                var NewDBFile = Path.Combine(Dir, "sbdb.xml");

                var XmlFile = new DataSet();
              

     //           var DefaultXML = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n<sbplanet>\n<System>Starbound Planet Scanner Export File</System>\n<Name></Name>\n<X>0</X>\n" +
     //"<Y>0</Y>\n<Biome></Biome>\n<Threat></Threat>\n<Notes>Exported from version " + " v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "</Notes>\n</sbplanet>";

                try
                {

                   // File.WriteAllText(NewDBFile, DefaultXML);

                    Directory.CreateDirectory(Dir + "\\planetpics");

                    XmlFile.ReadXml(DBFile,XmlReadMode.InferSchema);
                    XmlFile.Clear();


                    var PlanetPics = new List<string>();

                    for (int i = 0; i < Rows.Count; i++)
                    {
                      
                        // Sort may mess this up
                        // I guess I need an ID in there

                        var TestForRow = ((DataRowView)Rows[i].DataBoundItem);


                        if (TestForRow == null)
                            continue;

                        var TrueRow = (DataRow)  TestForRow.Row;

                      

                        XmlFile.Tables[0].ImportRow(TrueRow);
                        


                        // Should put regex here with more restrictions
                        var OName = Rows[i].Cells[1].Value.ToString().Replace("\\", "");
                        var Name = (("planetpics\\" + OName + ".png").Replace(" ", ""));

                        if (!String.IsNullOrEmpty(OName) && File.Exists(Name))
                        {
                            if (!File.Exists(Path.Combine(Dir, Name)))
                                File.Copy(Name, Path.Combine(Dir, Name));
                        }


                    }

                    XmlFile.WriteXml(NewDBFile);
                    XmlFile.WriteXmlSchema(Path.Combine(Dir, "schema.xml"));

             


                    XPathDocument myXPathDoc = new XPathDocument(NewDBFile);
                    XslCompiledTransform myXslTrans = new XslCompiledTransform();
                    myXslTrans.Load("SBDBtoHTML.xslt");
                    using (XmlTextWriter myWriter = new XmlTextWriter(Path.Combine(Dir, "HtmlTableOutput.html"), null)) { 
                    myXslTrans.Transform(myXPathDoc, null, myWriter);
                        }

                    if (File.Exists(FName))
                    {
                        File.Delete(FName);
                    }
                    System.IO.Compression.ZipFile.CreateFromDirectory(Dir, FName);

                    MessageBox.Show("Export Complete!");

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);

                }
                finally
                {
                    if (Directory.Exists(Dir))
                        Directory.Delete(Dir, true);
                }

            };

            BGW.RunWorkerCompleted += (o, arg) =>
            {
                exportbutton.Enabled = true;
               
            };


            BGW.RunWorkerAsync();


        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (importRowsDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var FName = importRowsDialog.FileName;

            BackgroundWorker BGW = new BackgroundWorker();

            importbutton.Enabled = false;

            dataGridView1.DataSource = null;


            BGW.DoWork += (o, args) =>
            {
                var Dir = GetTemporaryDirectory();
                var ExistingDB = Path.Combine(Dir, "sbdb.xml");

                try
                {

                  

                    System.IO.Compression.ZipFile.ExtractToDirectory(FName, Dir);

                    if (!File.Exists(ExistingDB))
                    {
                        MessageBox.Show("Invalid Zip file!");
                        Directory.Delete(Dir, true);
                        return;
                    }


                    var XmlFile = new DataSet();

                    XmlFile = ReadSBDB(ExistingDB, Path.Combine(Dir,"schema.xml"));


                    if (!Directory.Exists("planetpics"))
                    {
                        Directory.CreateDirectory("planetpics");
                    }


                    var PlanetPics = new List<string>();
                    var Rows = XmlFile.Tables[0].Rows;

                  

                    XmlData.Merge(XmlFile);

                    for (int i = 0; i < Rows.Count; i++)
                    {

                     //   XmlData.Tables[0].ImportRow(XmlFile.Tables[0].Rows[i]);


                        // Should put regex here with more restrictions
                        var OName = Rows[i].ItemArray[1].ToString().Replace("\\", "");
                        var Name = (("planetpics\\" + OName + ".png").Replace(" ", ""));

                        if (!String.IsNullOrEmpty(OName) && File.Exists(Path.Combine(Dir, Name)))
                        {
                            if (!File.Exists(Name))
                                File.Copy(Path.Combine(Dir, Name), Path.Combine(Name));
                        }


                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);

                }
                finally
                {
                    if (Directory.Exists(Dir))
                        Directory.Delete(Dir, true);
                }


               
            };

            BGW.RunWorkerCompleted += (o, arg) =>
            {
                importbutton.Enabled = true;

                

                dataGridView1.DataSource = XmlData.Tables[0];
                

            };


            BGW.RunWorkerAsync();

        }
    }
}
