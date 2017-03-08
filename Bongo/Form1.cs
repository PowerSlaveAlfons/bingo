using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;

namespace Bongo {
	public partial class Form1 : Form {

		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(String sClassName, String sAppName);

		Label[] labels = new Label[25];
		int selectedLabel = 25;
		int[] sheetLayout;

		Color[] colors = new Color[] { Color.LightGray, Color.LimeGreen, Color.DodgerBlue, Color.Red};

		int amountOfVeryHard;
		int amountOfHard;
		int amountOfMedium;

		int difficulty = 3;
		int seed = 0;

		List<XmlNode> theBingoBoard = new List<XmlNode>();

		private Hotkeys hotkeys;
		IntPtr thisWindow;

		[DllImport("user32.dll")]
		private static extern bool RegisterHotkey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

		[DllImport("user32.dll")]
		private static extern bool UnregisterHotkey(IntPtr hWnd, int id);





		public Form1() {
			InitializeComponent();
		}




		private void Form1_Load(object sender, EventArgs e) {

			thisWindow = FindWindow(null, "Bungo");
			hotkeys = new Hotkeys(thisWindow);
			hotkeys.RegisterHotkeys();

			string[] files = Directory.GetDirectories(@"Goals\");
			for (int i = 0; i < files.Length; i++) {
				comboBox1.Items.Add(files[i].Substring(6));
				files[i].Substring(7);
			}
			comboBox1.SelectedItem = comboBox1.Items[0];

			// Is there a better way of doing this? :P
			labels[0] = this.label1;
			labels[1] = this.label2;
			labels[2] = this.label3;
			labels[3] = this.label4;
			labels[4] = this.label5;
			labels[5] = this.label6;
			labels[6] = this.label7;
			labels[7] = this.label8;
			labels[8] = this.label9;
			labels[9] = this.label10;
			labels[10] = this.label11;
			labels[11] = this.label12;
			labels[12] = this.label13;
			labels[13] = this.label14;
			labels[14] = this.label15;
			labels[15] = this.label16;
			labels[16] = this.label17;
			labels[17] = this.label18;
			labels[18] = this.label19;
			labels[19] = this.label20;
			labels[20] = this.label21;
			labels[21] = this.label22;
			labels[22] = this.label23;
			labels[23] = this.label24;
			labels[24] = this.label25;
		}





		private void hideBoard(bool toHide) {
			foreach (Label l in labels) {
				l.Visible = !toHide;
			}
			unhideButton.Visible = toHide;
		}





		private void generateUID() {
			int uID;
			if (!string.IsNullOrEmpty(uIDBox.Text)) {
				uID = int.Parse(uIDBox.Text);
				seedDisplayBox.Text = uID.ToString();
				if (uID % 10000000 / 1000000 == 0 || uID % 10000000 / 1000000 >= 6) {
					trackBar1.Value = uID % 10000000 / 1000000;
				}
				else {
					trackBar1.Value = 3;
				}
				textBox1.Text = (uID % 100000).ToString();
			}
			else {
				if (string.IsNullOrEmpty(textBox1.Text)) {
					Random rand = new Random();
					seed = rand.Next(0, 100000);
				}
				else {
					seed = Math.Abs(int.Parse(textBox1.Text));
				}
				uID = difficulty * 1000000 + seed;
				seedDisplayBox.Text = uID.ToString();
			}
		}





		private void generateNewSheet() {

			generateUID();
			
			int uniqueCode = int.Parse(seedDisplayBox.Text);

			// Board info display on screen
			XmlDocument doc = new XmlDocument();
			doc.Load(@"Goals\\" + comboBox1.SelectedItem + "\\Goals.xml");
			XmlNode node = doc.DocumentElement.SelectSingleNode("info");
			if (node.Attributes["title"] == null || node.Attributes["description"] == null) {
				boardInfoBox.Text = "This goals file seems to have no title & description";
				goalInfoBox.Text = boardInfoBox.Text;
			}
			boardInfoBox.Text = node.Attributes["title"].InnerText + Environment.NewLine + Environment.NewLine + node.Attributes["description"].InnerText;
			goalInfoBox.Text = boardInfoBox.Text;

			// Sort goals by difficulty
			XmlNodeList goalsAll = doc.DocumentElement.GetElementsByTagName("goal");
			List<XmlNode> goalsVeryHard = new List<XmlNode>();
			List<XmlNode> goalsHard = new List<XmlNode>();
			List<XmlNode> goalsMedium = new List<XmlNode>();
			List<XmlNode> goalsEasy = new List<XmlNode>();

			theBingoBoard.Clear();

			foreach (XmlNode goal in goalsAll) {
				if (goal.Attributes["difficulty"] != null)
					switch (goal.Attributes["difficulty"].InnerText) {
						case "0":
							goalsEasy.Add(goal);
							break;
						case "1":
							goalsMedium.Add(goal);
							break;
						case "2":
							goalsHard.Add(goal);
							break;
						case "3":
							goalsVeryHard.Add(goal);
							break;
						default:
							boardInfoBox.Text = "Error: The goal '" + goal.Attributes["name"].InnerText + "' has no difficulty set!" + Environment.NewLine + boardInfoBox.Text;
							goalInfoBox.Text = boardInfoBox.Text;
							break;
					}
				else {
					boardInfoBox.Text = "Error: The goal '" + goal.Attributes["name"].InnerText + "' has no difficulty set!" + Environment.NewLine + boardInfoBox.Text;
					goalInfoBox.Text = boardInfoBox.Text;
				}
			}
			//Debug.WriteLine(nodeListA[i].Attributes["name"].InnerText);


			/*
            * As well as the random 5 digit seed used for randomizing, the unique identifier also contains data for all other settings.
            * Like difficulty, board layout, etc. The format is listed below. I use modulo and division to separate this digit from the rest.
            * Remove previous digits by taking modulo 10^(N+1) where N = the digit I need, starting from the last
            * Then divide by 10^N to remove the latter digits
            *
            * 
            * unique code format: DLXXXXX
            * XXXXX = seed for RNG.
            * L = Layout (1 = set, rest = random)
            * D = Difficulty (easy 1 - 5 hard, rest = treated as 3
            * Future ideas:
            * G = Glitchiness (1 = can be done intended strats, 2 = Requires glitches
            * D = Duration (1 = short, 5 = long, etc)
            */

			int seed = uniqueCode % 100000;
			Random rand = new Random(seed);
			
			// Determine the board layout
			if (uniqueCode % 1000000 / 100000 == 1) {
				// fixed bingo layout
				sheetLayout = new int[] {
					1, 2, 0, 2, 1,
					2, 0, 1, 0, 2,
					0, 1, 3, 1, 0,
					2, 0, 1, 0, 2,
					1, 2, 0, 2, 1
				};
			}
			else {
				// random bingo layout
				sheetLayout = new int[25];
			}


			// Determine the difficulty
			int difficulty = uniqueCode % 10000000 / 1000000;
			switch (difficulty) {
				case 1:
					// Very Easy
					amountOfVeryHard = 0;
					amountOfHard = 0;
					amountOfMedium = 0;
					break;
				case 2:
					// Easy
					amountOfVeryHard = 0;
					amountOfHard = Math.Min(2,goalsHard.Count);
					amountOfMedium = Math.Min(5, goalsMedium.Count);
					break;
				case 4:
					// Hard
					amountOfVeryHard = Math.Min(rand.Next(0, 2), goalsVeryHard.Count);
					amountOfHard = Math.Min(rand.Next(3, 6), goalsHard.Count);
					amountOfMedium = Math.Min(rand.Next(8, 11), goalsMedium.Count);
					break;
				case 5:
					// Hardest
					amountOfVeryHard = Math.Min(3, goalsVeryHard.Count);
					amountOfHard = Math.Min(6, goalsHard.Count);
					amountOfMedium = Math.Min(13, goalsMedium.Count);
					break;
				default:
					// Moderate (default)
					amountOfVeryHard = Math.Min(rand.Next(0, 3) / 2, goalsVeryHard.Count);
					amountOfHard = Math.Min(rand.Next(2, 5), goalsHard.Count);
					amountOfMedium = Math.Min(rand.Next(6, 9), goalsMedium.Count);
					break;
			}
			if (amountOfHard + amountOfMedium + amountOfVeryHard < 25 - goalsEasy.Count) {
				boardInfoBox.Text = "Uh oh, there seem to not be enough easy goals to create a board on this difficulty";
				goalInfoBox.Text = boardInfoBox.Text;
				return;
			}


			// Shuffle the tiles 1-25 in a random order
			List<int> bingoBoardTiles = new List<int>(Enumerable.Range(0, 24));
			Random bingoBoardRandomizer = new Random(seed);
			List<int> bingoBoardShuffledTiles = new List<int>();

			while (bingoBoardTiles.Count > 0) {
				int bingoTile = bingoBoardRandomizer.Next(bingoBoardTiles.Count);
				bingoBoardShuffledTiles.Add(bingoBoardTiles[bingoTile]);
				bingoBoardTiles.RemoveAt(bingoTile);
			}

			for (int i = 0; i < 25; i++) {
				if (i < amountOfVeryHard) {
					if (sheetLayout[bingoBoardShuffledTiles[i]] == 0) {
						sheetLayout[bingoBoardShuffledTiles[i]] = 3;
					}
				}
				else if (i < amountOfVeryHard + amountOfHard) {
					if (sheetLayout[bingoBoardShuffledTiles[i]] == 0) {
						sheetLayout[bingoBoardShuffledTiles[i]] = 2;
					}
				}
				else if (i < amountOfVeryHard + amountOfHard + amountOfMedium) {
					if (sheetLayout[bingoBoardShuffledTiles[i]] == 0) {
						sheetLayout[bingoBoardShuffledTiles[i]] = 1;
					}
				}
			}
			// Update the board with numbers to see if it works (it does)	// TO BE DELETED
			for (int i = 0; i < 25; i++) {                                  // TO BE DELETED
				labels[i].Text = sheetLayout[i].ToString();                 // TO BE DELETED
			}                                                               // TO BE DELETED


			for (int i = 0; i < 25; i++) {
				switch (sheetLayout[i]) {
					case 1:
						// medium goal
						int goalToAddM = rand.Next(goalsMedium.Count);
						theBingoBoard.Add(goalsMedium[goalToAddM]);
						labels[i].Text = goalsMedium[goalToAddM].Attributes["name"].InnerText;
						goalsMedium.RemoveAt(goalToAddM);
						break;
					case 2:
						//hard goal
						int goalToAddH = rand.Next(goalsHard.Count);
						theBingoBoard.Add(goalsHard[goalToAddH]);
						labels[i].Text = goalsHard[goalToAddH].Attributes["name"].InnerText;
						goalsHard.RemoveAt(goalToAddH);
						break;
					case 3:
						//very hard goal
						int goalToAddV = rand.Next(goalsVeryHard.Count);
						theBingoBoard.Add(goalsVeryHard[goalToAddV]);
						labels[i].Text = goalsVeryHard[goalToAddV].Attributes["name"].InnerText;
						goalsVeryHard.RemoveAt(goalToAddV);
						break;
					default:
						//easy goal
						int goalToAddE = rand.Next(goalsEasy.Count);
						theBingoBoard.Add(goalsEasy[goalToAddE]);
						labels[i].Text = goalsEasy[goalToAddE].Attributes["name"].InnerText;
						goalsEasy.RemoveAt(goalToAddE);
						break;
				}
			}
		}





		#region board tile controls
		// Upon clicking on a tile, either select it, or change its color if it already is.
		private void mouseTileClick(Label clickedLabel, bool leftMouseButton) {
			int clickedLabelIndex = Array.FindIndex(labels, item => item == clickedLabel);
			if (selectedLabel == clickedLabelIndex) {
				if (leftMouseButton) {
					int colorIndexR = Array.FindIndex(colors, item => item == clickedLabel.BackColor);
					clickedLabel.BackColor = colors[(colorIndexR + 1) % colors.Length];
				}
				else {
					int colorIndexL = Array.FindIndex(colors, item => item == clickedLabel.BackColor);
					clickedLabel.BackColor = colors[(colorIndexL + colors.Length - 1) % colors.Length];
				}
			}
			else {
				if (selectedLabel != 25) {
					Point locationToPutA = new Point(labels[selectedLabel].Location.X + 5, labels[selectedLabel].Location.Y + 5);
					labels[selectedLabel].Location = locationToPutA;
					Font fontToPutA = new Font(labels[selectedLabel].Font, FontStyle.Regular);
					labels[selectedLabel].Font = fontToPutA;
					Size sizeToPutA = new Size(labels[selectedLabel].Size.Height - 10, labels[selectedLabel].Size.Width - 10);
					labels[selectedLabel].Size = sizeToPutA;
				}
				selectedLabel = clickedLabelIndex;
				Point locationToPutB = new Point(labels[selectedLabel].Location.X - 5, labels[selectedLabel].Location.Y - 5);
				labels[selectedLabel].Location = locationToPutB;
				Font fontToPutB = new Font(labels[selectedLabel].Font, FontStyle.Bold);
				labels[selectedLabel].Font = fontToPutB;
				Size sizeToPutB = new Size(labels[selectedLabel].Size.Height + 10, labels[selectedLabel].Size.Width + 10);
				labels[selectedLabel].Size = sizeToPutB;
				if (selectedLabel <= theBingoBoard.Count) {
					if (theBingoBoard[selectedLabel].Attributes["description"] != null) {
						goalInfoBox.Text = theBingoBoard[selectedLabel].Attributes["description"].InnerText;
					}
					else {
						goalInfoBox.Text = "";
					}
				}
			}
		}

		private void hotkeyTileMove(int action) {
			if (action <= 50) {
				if (selectedLabel != 25) {
					Point locationToPutA = new Point(labels[selectedLabel].Location.X + 5, labels[selectedLabel].Location.Y + 5);
					labels[selectedLabel].Location = locationToPutA;
					Font fontToPutA = new Font(labels[selectedLabel].Font, FontStyle.Regular);
					labels[selectedLabel].Font = fontToPutA;
					Size sizeToPutA = new Size(labels[selectedLabel].Size.Height - 10, labels[selectedLabel].Size.Width - 10);
					labels[selectedLabel].Size = sizeToPutA;
				}
				else {
					selectedLabel = 0;
				}
				selectedLabel = (selectedLabel + 25 + action) % 25;
				Point locationToPutB = new Point(labels[selectedLabel].Location.X - 5, labels[selectedLabel].Location.Y - 5);
				labels[selectedLabel].Location = locationToPutB;
				Font fontToPutB = new Font(labels[selectedLabel].Font, FontStyle.Bold);
				labels[selectedLabel].Font = fontToPutB;
				Size sizeToPutB = new Size(labels[selectedLabel].Size.Height + 10, labels[selectedLabel].Size.Width + 10);
				labels[selectedLabel].Size = sizeToPutB;
				if (selectedLabel <= theBingoBoard.Count) {
					if (theBingoBoard[selectedLabel].Attributes["description"] != null) {
						goalInfoBox.Text = theBingoBoard[selectedLabel].Attributes["description"].InnerText;
					}
					else {
						goalInfoBox.Text = "";
					}
				}
			}
		}

		private void hotkeyTileColorize(int action) {
			int colorIndexL = Array.FindIndex(colors, item => item == labels[selectedLabel].BackColor);
			labels[selectedLabel].BackColor = colors[(colorIndexL + colors.Length + action) % colors.Length];
		}

		protected override void WndProc(ref Message keyPressed) {
			if (keyPressed.Msg == 0x0312) {
				int key = keyPressed.WParam.ToInt32();
				if (!unhideButton.Visible) {
					switch (key) {
						default:
							break;
						case 0: //up
							hotkeyTileMove(20);
							break;
						case 1: //downm
							hotkeyTileMove(5);
							break;
						case 2: //left
							hotkeyTileMove(24);
							break;
						case 3: //rght
							hotkeyTileMove(1);
							break;
						case 4: //colorback
							hotkeyTileColorize(-1);
							break;
						case 5: //colornext
							hotkeyTileColorize(1);
							break;
					}
				}
				else if (key == 6) {
					hideBoard(false);
				}
			}
			base.WndProc(ref keyPressed);
		}
		#endregion	



		private void button1_Click(object sender, EventArgs e) {
			hideBoard(checkBox1.Checked);
			generateNewSheet();
			tabControl1.SelectedTab = tabPage1;
		}

		private void hideButton_Click(object sender, EventArgs e) {
			hideBoard(false);
		}

		private void tile_Click(object sender, MouseEventArgs e) {
			Label clickedLabel = sender as Label;
			mouseTileClick(clickedLabel, e.Button == MouseButtons.Left);
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
			hotkeys.UnregisterHotkeys();
		}

		private void textBox1_TextChanged(object sender, EventArgs e) {
			int a;
			if (!int.TryParse(textBox1.Text, out a)) {
				textBox1.Clear();
			}
			generateUID();
		}

		private void uIDBox_TextChanged(object sender, EventArgs e) {
			int a;
			if (!int.TryParse(textBox1.Text, out a)) {
				uIDBox.Clear();
			}
			generateUID();
		}

		private void trackBar1_Scroll(object sender, EventArgs e) {
			difficulty = trackBar1.Value;
			generateUID();
		}
	}
}
