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


namespace Bongo {
	public partial class Form1 : Form {

		Label[] labels = new Label[25];
		int[] sheetLayout;

		int amountOfVeryHard;
		int amountOfHard;
		int amountOfMedium;

		List<XmlNode> theBingoBoard = new List<XmlNode>();

		public Form1() {
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e) {
			//https://github.com/Joshimuz/mcbingo/blob/master/bingo.js

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


		private void generateNewSheet(int uniqueCode) {

			// Board info display on screen
			XmlDocument doc = new XmlDocument();
			doc.Load(@"Goals\\" + comboBox1.SelectedItem + "\\Goals.xml");
			XmlNode node = doc.DocumentElement.SelectSingleNode("info");
			if (node.Attributes["title"] == null || node.Attributes["description"] == null) {
				boardInfoBox.Text = "This goals file seems to have no title & description";
			}
			boardInfoBox.Text = node.Attributes["title"].InnerText + Environment.NewLine + Environment.NewLine + node.Attributes["description"].InnerText;

			// Sort goals by difficulty
			XmlNodeList goalsAll = doc.DocumentElement.GetElementsByTagName("goal");
			List<XmlNode> goalsVeryHard = new List<XmlNode>();
			List<XmlNode> goalsHard = new List<XmlNode>();
			List<XmlNode> goalsMedium = new List<XmlNode>();
			List<XmlNode> goalsEasy = new List<XmlNode>();

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
							break;
					}
				else {
					boardInfoBox.Text = "Error: The goal '" + goal.Attributes["name"].InnerText + "' has no difficulty set!" + Environment.NewLine + boardInfoBox.Text;
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
				Debug.WriteLine(sheetLayout[i]);
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
						int goalToAddV = rand.Next(goalsVeryHard.Count);
						theBingoBoard.Add(goalsVeryHard[goalToAddV]);
						labels[i].Text = goalsVeryHard[goalToAddV].Attributes["name"].InnerText;
						goalsVeryHard.RemoveAt(goalToAddV);
						break;
					default:
						int goalToAddE = rand.Next(goalsEasy.Count);
						theBingoBoard.Add(goalsEasy[goalToAddE]);
						labels[i].Text = goalsEasy[goalToAddE].Attributes["name"].InnerText;
						goalsEasy.RemoveAt(goalToAddE);
						break;
				}
			}
		}



		private void button1_Click(object sender, EventArgs e) {
			generateNewSheet(int.Parse(seedDisplayBox.Text));
		}


	}
}
