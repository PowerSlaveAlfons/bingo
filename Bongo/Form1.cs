﻿using System;
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

		Color[] colors = new Color[] { Color.LightGray, Color.DodgerBlue, Color.LimeGreen, Color.Red };

		bool bingoActive = false;

		int amountOfVeryHard;
		int amountOfHard;
		int amountOfMedium;

		int amountOfLandMine;
		int amountOfVeryLong;
		int amountOfLong;
		int amountOfShort;
		int amountOfVeryShort;

		int length = 4;
		int difficulty = 3;
		int seed = 0;

		List<XmlNode> theBingoBoard = new List<XmlNode>();

		List<XmlNode> goalsVeryHard = new List<XmlNode>();
		List<XmlNode> goalsHard = new List<XmlNode>();
		List<XmlNode> goalsMedium = new List<XmlNode>();
		List<XmlNode> goalsEasy = new List<XmlNode>();

		private Hotkeys hotkeys;
		IntPtr thisWindow;

		public uint hotkeyU = 0;
		public uint hotkeyD = 0;
		public uint hotkeyL = 0;
		public uint hotkeyR = 0;
		public uint hotkeyP = 0;
		public uint hotkeyN = 0;
		public uint hotkeyH = 0;
		public uint hotkeyT = 0;
		public uint modifierU = 0;
		public uint modifierD = 0;
		public uint modifierL = 0;
		public uint modifierR = 0;
		public uint modifierP = 0;
		public uint modifierN = 0;
		public uint modifierH = 0;
		public uint modifierT = 0;

		Dictionary<string, uint> KeyCodesCustom = new Dictionary<string, uint> {
			{"None", 0x00 },
			{ "Backspace", 0x08 },
			{"Tab", 0x09},
			{"Enter", 0x0D},
			{"Pause", 0x13},
			{"CapsLock", 0x14},
			{"Escape", 0x1B},
			{"Space", 0x20},
			{"PageUp", 0x21},
			{"PageDn", 0x22},
			{"End", 0x23},
			{"Home", 0x24},
			{"LeftArrow", 0x25},
			{"UpArrow", 0x26},
			{"RightArrow", 0x27},
			{"DownArrow", 0x28},
			{"PrtScr", 0x2C},
			{"Insert", 0x2D},
			{"Delete", 0x2E},
			{"0", 0x30},
			{"1", 0x31},
			{"2", 0x32},
			{"3", 0x33},
			{"4", 0x34},
			{"5", 0x35},
			{"6", 0x36},
			{"7", 0x37},
			{"8", 0x38},
			{"9", 0x39},

			{"A", 0x41},
			{"B", 0x42},
			{"C", 0x43},
			{"D", 0x44},
			{"E", 0x45},
			{"F", 0x46},
			{"G", 0x47},
			{"H", 0x48},
			{"I", 0x49},
			{"J", 0x4A},
			{"K", 0x4B},
			{"L", 0x4C},
			{"M", 0x4D},
			{"N", 0x4E},
			{"O", 0x4F},
			{"P", 0x50},
			{"Q", 0x51},
			{"R", 0x52},
			{"S", 0x53},
			{"T", 0x54},
			{"U", 0x55},
			{"V", 0x56},
			{"W", 0x57},
			{"X", 0x58},
			{"Y", 0x59},
			{"Z", 0x5A},

			{"Apps", 0x5D},
			{"KP_0", 0x60},
			{"KP_1", 0x61},
			{"KP_2", 0x62},
			{"KP_3", 0x63},
			{"KP_4", 0x64},
			{"KP_5", 0x65},
			{"KP_6", 0x66},
			{"KP_7", 0x67},
			{"KP_8", 0x68},
			{"KP_9", 0x69},
			{"KP_*", 0x6A},
			{"KP_+", 0x6B},
			{"KP_-", 0x6D},
			{"KP_.", 0x6E},
			{"KP_/", 0x6F},


			{"F1", 0x70},
			{"F2", 0x71},
			{"F3", 0x72},
			{"F4", 0x73},
			{"F5", 0x74},
			{"F6", 0x75},
			{"F7", 0x76},
			{"F8", 0x77},
			{"F9", 0x78},
			{"F10", 0x79},
			{"F11", 0x7A},
			{"F12", 0x7B},
			{"F13", 0x7C},
			{"F14", 0x7D},
			{"F15", 0x7E},
			{"F16", 0x7F},
			{"F17", 0x80},
			{"F18", 0x81},
			{"F19", 0x82},
			{"F20", 0x83},
			{"F21", 0x84},
			{"F22", 0x85},
			{"F23", 0x86},
			{"F24", 0x87},

			{"NumLock", 0x90 },
			{"ScrollLock",0x91 },

			{":",0xBA },
			{"+",0xBB },
			{",",0xBC },
			{"-",0xBD },
			{".",0xBE },
			{"/",0xBF },
			{"~",0xC0 },

			{"[",0xDB },
			{"\\",0xDC },
			{"]",0xDD },
			{"'",0xDE },

		};


		[DllImport("user32.dll")]
		private static extern bool RegisterHotkey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

		[DllImport("user32.dll")]
		private static extern bool UnregisterHotkey(IntPtr hWnd, int id);





		public Form1() {
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e) {
			//Lookup goal directories and put them in the goal selector dropdown
			string[] files = Directory.GetDirectories(@"Goals\");
			for (int i = 0; i < files.Length; i++) {
				comboBox1.Items.Add(files[i].Substring(6));
				files[i].Substring(7);
			}
			comboBox1.SelectedItem = comboBox1.Items[0];
			//Register hotkeys
			thisWindow = FindWindow(null, "Bungo");
			hotkeys = new Hotkeys(thisWindow);
			#region hotkey combobox content assignment
			comboU.DataSource = new BindingSource(KeyCodesCustom, null);
			comboU.DisplayMember = "Key";
			comboU.ValueMember = "Value";
			comboD.DataSource = new BindingSource(KeyCodesCustom, null);
			comboD.DisplayMember = "Key";
			comboD.ValueMember = "Value";
			comboL.DataSource = new BindingSource(KeyCodesCustom, null);
			comboL.DisplayMember = "Key";
			comboL.ValueMember = "Value";
			comboR.DataSource = new BindingSource(KeyCodesCustom, null);
			comboR.DisplayMember = "Key";
			comboR.ValueMember = "Value";
			comboP.DataSource = new BindingSource(KeyCodesCustom, null);
			comboP.DisplayMember = "Key";
			comboP.ValueMember = "Value";
			comboN.DataSource = new BindingSource(KeyCodesCustom, null);
			comboN.DisplayMember = "Key";
			comboN.ValueMember = "Value";
			comboH.DataSource = new BindingSource(KeyCodesCustom, null);
			comboH.DisplayMember = "Key";
			comboH.ValueMember = "Value";
			comboT.DataSource = new BindingSource(KeyCodesCustom, null);
			comboT.DisplayMember = "Key";
			comboT.ValueMember = "Value";
			#endregion	
			configXMLRead();
			hotkeys.RegisterHotkeys(hotkeyU, hotkeyD, hotkeyL, hotkeyR, hotkeyP, hotkeyN, hotkeyH, hotkeyT, modifierU, modifierD, modifierL, modifierR, modifierP, modifierN, modifierH, modifierT);
			#region label assignment // Is there a better way of doing this? :P
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
			#endregion
		}

		private void configXMLRead() {
			// Load config
			XmlDocument docConfig = new XmlDocument();
			docConfig.Load(@"BingoConfig.xml");
			// Restore last game
			XmlNode node = docConfig.DocumentElement.SelectSingleNode("lastgame");
			comboBox1.SelectedItem = node.InnerText;
			// Restore hotkeys
			XmlNode node2 = docConfig.DocumentElement.SelectSingleNode("hotkeys");
			comboU.SelectedValue = UInt32.Parse(node2.Attributes["U"].InnerText);
			comboD.SelectedValue = UInt32.Parse(node2.Attributes["D"].InnerText);
			comboL.SelectedValue = UInt32.Parse(node2.Attributes["L"].InnerText);
			comboR.SelectedValue = UInt32.Parse(node2.Attributes["R"].InnerText);
			comboP.SelectedValue = UInt32.Parse(node2.Attributes["P"].InnerText);
			comboN.SelectedValue = UInt32.Parse(node2.Attributes["N"].InnerText);
			comboH.SelectedValue = UInt32.Parse(node2.Attributes["H"].InnerText);
			comboT.SelectedValue = UInt32.Parse(node2.Attributes["T"].InnerText);
			// Restore modifiers
			XmlNode node3 = docConfig.DocumentElement.SelectSingleNode("modifiers");

			altU.Checked = Int32.Parse(node3.Attributes["U"].InnerText) % 2 == 1;
			ctrlU.Checked = Int32.Parse(node3.Attributes["U"].InnerText) % 4 >= 2;
			shiftU.Checked = Int32.Parse(node3.Attributes["U"].InnerText) % 8 >= 4;
			winU.Checked = Int32.Parse(node3.Attributes["U"].InnerText) >= 8;

			altD.Checked = Int32.Parse(node3.Attributes["D"].InnerText) % 2 == 1;
			ctrlD.Checked = Int32.Parse(node3.Attributes["D"].InnerText) % 4 >= 2;
			shiftD.Checked = Int32.Parse(node3.Attributes["D"].InnerText) % 8 >= 4;
			winD.Checked = Int32.Parse(node3.Attributes["D"].InnerText) >= 8;

			altL.Checked = Int32.Parse(node3.Attributes["L"].InnerText) % 2 == 1;
			ctrlL.Checked = Int32.Parse(node3.Attributes["L"].InnerText) % 4 >= 2;
			shiftL.Checked = Int32.Parse(node3.Attributes["L"].InnerText) % 8 >= 4;
			winL.Checked = Int32.Parse(node3.Attributes["L"].InnerText) >= 8;

			altR.Checked = Int32.Parse(node3.Attributes["R"].InnerText) % 2 == 1;
			ctrlR.Checked = Int32.Parse(node3.Attributes["R"].InnerText) % 4 >= 2;
			shiftR.Checked = Int32.Parse(node3.Attributes["R"].InnerText) % 8 >= 4;
			winR.Checked = Int32.Parse(node3.Attributes["R"].InnerText) >= 8;

			altP.Checked = Int32.Parse(node3.Attributes["P"].InnerText) % 2 == 1;
			ctrlP.Checked = Int32.Parse(node3.Attributes["P"].InnerText) % 4 >= 2;
			shiftP.Checked = Int32.Parse(node3.Attributes["P"].InnerText) % 8 >= 4;
			winP.Checked = Int32.Parse(node3.Attributes["P"].InnerText) >= 8;

			altN.Checked = Int32.Parse(node3.Attributes["N"].InnerText) % 2 == 1;
			ctrlN.Checked = Int32.Parse(node3.Attributes["N"].InnerText) % 4 >= 2;
			shiftN.Checked = Int32.Parse(node3.Attributes["N"].InnerText) % 8 >= 4;
			winN.Checked = Int32.Parse(node3.Attributes["N"].InnerText) >= 8;

			altH.Checked = Int32.Parse(node3.Attributes["H"].InnerText) % 2 == 1;
			ctrlH.Checked = Int32.Parse(node3.Attributes["H"].InnerText) % 4 >= 2;
			shiftH.Checked = Int32.Parse(node3.Attributes["H"].InnerText) % 8 >= 4;
			winH.Checked = Int32.Parse(node3.Attributes["H"].InnerText) >= 8;

			altT.Checked = Int32.Parse(node3.Attributes["T"].InnerText) % 2 == 1;
			ctrlT.Checked = Int32.Parse(node3.Attributes["T"].InnerText) % 4 >= 2;
			shiftT.Checked = Int32.Parse(node3.Attributes["T"].InnerText) % 8 >= 4;
			winT.Checked = Int32.Parse(node3.Attributes["T"].InnerText) >= 8;
		}
		
		private void hideBoard(bool toHide) {
			foreach (Label l in labels) {
				l.Visible = !toHide;
			}
			unhideButton.Visible = toHide;
		}

		private void generateUID() {
			int uID;
			// Specific uID entered, so set all the sliders and checkboxes in the settings to match
			if (!string.IsNullOrEmpty(uIDBox.Text)) {
				uID = int.Parse(uIDBox.Text);
				// Set seed settings
				seedDisplayBox.Text = uID.ToString();
				// Set difficulty settings
				if (uID % 10000000 / 1000000 == 0) {
					checkBoxDiff.Checked = true;
					trackBar1.Enabled = false;
				}
				else if (uID % 10000000 / 1000000 <= 5) {
					trackBar1.Enabled = true;
					checkBoxDiff.Checked = false;
					trackBar1.Value = uID % 10000000 / 1000000;
				}
				else {
					trackBar1.Enabled = true;
					checkBoxDiff.Checked = false;
					trackBar1.Value = 3;
				}
				// Set length settings
				if (uID % 100000000 / 10000000 == 0) {
					checkBoxLength.Checked = true;
					trackBar2.Enabled = false;
				}
				else if (uID % 100000000 / 10000000 <= 7) {
					trackBar2.Enabled = true;
					checkBoxLength.Checked = false;
					trackBar2.Value = uID % 100000000 / 10000000;
				}
				else {
					trackBar2.Enabled = true;
					checkBoxLength.Checked = false;
					trackBar2.Value = 4;
				}
				// Print uid in box
				textBox1.Text = (uID % 100000).ToString();
			}
			// User creates a new board using the settings, so turn these settings into an uID.
			else {
				if (string.IsNullOrEmpty(textBox1.Text)) {
					Random rand = new Random();
					seed = rand.Next(0, 100000);
				}
				else {
					seed = Math.Abs(int.Parse(textBox1.Text));
				}
				uID = length * 10000000 + difficulty * 1000000 + seed;
				seedDisplayBox.Text = uID.ToString();
			}
		}

		// Sort the goals in the XML by difficulty/length, etc and print stuff from the XML on the screen
		// This essentially processes the entire goals.xml and makes the info ready to be put on the board.
		private void initialXMLRead() {
			#region print version + info
			// Board info display on screen
			XmlDocument docGoals = new XmlDocument();
			docGoals.Load(@"Goals\\" + comboBox1.SelectedItem + "\\Goals.xml");
			XmlNode node = docGoals.DocumentElement.SelectSingleNode("info");
			// Print title + description + rules + whatever else they put
			if (node.Attributes["title"] == null || node.Attributes["description"] == null) {
				boardInfoBox.Text = "This goals file seems to have no title & description";
				goalInfoBox.Text = boardInfoBox.Text;
			}
			else {
				boardInfoBox.Text = "Goals title: " + node.Attributes["title"].InnerText + Environment.NewLine + Environment.NewLine + "Goals description & rules:" + Environment.NewLine + node.Attributes["description"].InnerText;
			}
			// Print version on top of that
			node = docGoals.DocumentElement.SelectSingleNode("version");
			if (node.Attributes["version"] == null) {
				boardInfoBox.Text = "Unknown goals version" + Environment.NewLine + boardInfoBox.Text;
			}
			else {
				boardInfoBox.Text = "Goals version: " + node.Attributes["version"].InnerText + Environment.NewLine + boardInfoBox.Text;
			}
			// Put all that was printed in the goal info box too
			goalInfoBox.Text = boardInfoBox.Text;
			#endregion

			// Sort goals by difficulty and length etc
			XmlNodeList goalsAll = docGoals.DocumentElement.GetElementsByTagName("goal");

			foreach (XmlNode goal in goalsAll) {
				bool cont = true;       // If the length of the goal is too long/short, we don't need to sit through the difficulty check anymore.
				if (goal.Attributes["length"] != null && !checkBoxLength.Checked) {
					// Goal has length configured and length is factored in
					switch (goal.Attributes["length"].InnerText) {
						case "5":
							if (amountOfLandMine == 0) {
								cont = false;
							}
							break;
						case "4":
							if (amountOfVeryLong == 0) {
								cont = false;
							}
							break;
						case "3":
							if (amountOfLong == 0) {
								cont = false;
							}
							break;
						case "2":
							if (amountOfShort == 0) {
								cont = false;
							}
							break;
						case "1":
							if (amountOfVeryShort == 0) {
								cont = false;
							}
							break;
						case "0":
							if (amountOfLandMine + amountOfVeryLong + amountOfLong + amountOfShort + amountOfVeryShort == 25) {
								cont = false;
							}
							break;
						default:
							boardInfoBox.Text = "Error: The goal '" + goal.Attributes["name"].InnerText + "' has no length set!" + Environment.NewLine + boardInfoBox.Text;
							goalInfoBox.Text = boardInfoBox.Text;
							cont = false;
							break;
					}
				}
				else if (!checkBoxLength.Checked) {
					//Length is factored in, but the goal doesn't have length set
					boardInfoBox.Text = "Error: The goal '" + goal.Attributes["name"].InnerText + "' has no length set!" + Environment.NewLine + boardInfoBox.Text;
					goalInfoBox.Text = boardInfoBox.Text;
					cont = false;
				}
				// If length isn't factored in, just continue to difficulty check
				if (goal.Attributes["difficulty"] != null && cont == true && !checkBoxDiff.Checked) {
					Debug.WriteLine("executing");

					// Difficulty is factored in, length of the goal is valid, and goal has difficulty set
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
				}
				else if (checkBoxDiff.Checked && cont == true) {
					// difficulty isn't factored in anyway, and the length of the goal is valid
					goalsEasy.Add(goal);
				}
				else if (cont == true) {
					// Goal doesn't have difficulty set, and difficulty is factored in, and the length of the goal is valid
					boardInfoBox.Text = "Error: The goal '" + goal.Attributes["name"].InnerText + "' has no difficulty set!" + Environment.NewLine + boardInfoBox.Text;
					goalInfoBox.Text = boardInfoBox.Text;
				}
				// If length of goal isn't valid (cont == false) then just do nothing
			}
		}
		
		private void GenerateNewSheet() {
			bingoActive = true;
			/*
			* As well as the random 5 digit seed used for randomizing, the unique identifier also contains data for all other settings.
			* Like difficulty, board layout, etc. The format is listed below. I use modulo and division to separate this digit from the rest.
			* Remove previous digits by taking modulo 10^(N+1) where N = the digit I need, starting from the last
			* Then divide by 10^N to remove the latter digits
			*
			* 
			* unique code format: LDBXXXXX
			* XXXXX = seed for RNG.
			* B = Board Layout (1 = set, rest = random)			// This isn't actually used lol
			* D = Difficulty (easy 1 - 5 hard, rest = treated as 3 , except for 0 which means 'just put goals randomly without looking at difficulty'
			* L = Length (1 = short, 7 = long, etc), rest = treated as 4, except for 0 which means the same as it does with difficulty.
			* Future ideas:
			* G = Glitchiness (1 = can be done intended strats, 2 = Requires glitches
			*/

			// Cleanup the board ya numpty
			goalsEasy.Clear();
			goalsMedium.Clear();
			goalsHard.Clear();
			goalsVeryHard.Clear();
			theBingoBoard.Clear();
			resetBoardColors();

			// Generate an UID, then parse stuff from the textbox.
			// I should make generateUID return the int with the uID instead lol
			generateUID();
			int uniqueCode = int.Parse(seedDisplayBox.Text);

			// Determine the seed
			int seed = uniqueCode % 100000;
			Random rand = new Random(seed);

			// Determine the length
			int length = uniqueCode % 100000000 / 10000000;
			switch (length) {
				case 1:
					// Very Short
					amountOfLandMine = 0;
					amountOfVeryLong = 0;
					amountOfLong = 0;
					amountOfShort = 0;
					amountOfVeryShort = 0;
					break;
				case 2:
					// Short
					amountOfLandMine = 0;
					amountOfVeryLong = 0;
					amountOfLong = 0;
					amountOfShort = 0;
					amountOfVeryShort = 12;
					break;
				case 3:
					// Somewhat Short
					amountOfLandMine = 0;
					amountOfVeryLong = 0;
					amountOfLong = 0;
					amountOfShort = 5;
					amountOfVeryShort = 10;
					break;
				case 0:
					// Disregarded
					amountOfLandMine = -1;
					amountOfVeryLong = -1;
					amountOfLong = -1;
					amountOfShort = -1;
					amountOfVeryShort = -1;
					break;
				case 5:
					// Somewhat Long
					amountOfLandMine = 0;
					amountOfVeryLong = 5;
					amountOfLong = 5;
					amountOfShort = 5;
					amountOfVeryShort = 5;
					break;
				case 6:
					// Long
					amountOfLandMine = 4;
					amountOfVeryLong = 5;
					amountOfLong = 5;
					amountOfShort = 5;
					amountOfVeryShort = 3;
					break;
				case 7:
					// Very Long
					amountOfLandMine = 10;
					amountOfVeryLong = 10;
					amountOfLong = 5;
					amountOfShort = 0;
					amountOfVeryShort = 0;
					break;
				default:
					// Normal length
					amountOfLandMine = 0;
					amountOfVeryLong = 0;
					amountOfLong = 4;
					amountOfShort = 7;
					amountOfVeryShort = 7;
					break;
			}

			// Now that we've determined the length, process all the goals. It will throw out all goals that are too short or too long.
			initialXMLRead();

			#region Determine the board layout (unused)
			if (uniqueCode % 1000000 / 100000 == 1) {
				// fixed bingo layout
				sheetLayout = new int[] {
					1, 2, 0, 2, 1,					// 1 5 3 4 2
					2, 0, 1, 0, 2,					// 4 2 1 5 3
					0, 1, 3, 1, 0,					// 5 3 4 2 1
					2, 0, 1, 0, 2,					// 2 1 5 3 4
					1, 2, 0, 2, 1					// 3 4 2 1 5
				};
			}
			else {
				// random bingo layout
				sheetLayout = new int[25];
			}
			#endregion

			#region Determine the difficulty
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
					amountOfHard = Math.Min(2, goalsHard.Count);
					amountOfMedium = Math.Min(5, goalsMedium.Count);
					break;
				case 0:
					amountOfVeryHard = 0;
					amountOfHard = 0;
					amountOfMedium = 0;
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

			// Check if there's enough goals left after all the filtering nonsense I've done to actually make a board still.
			if (amountOfHard + amountOfMedium + amountOfVeryHard < 25 - goalsEasy.Count) {
				bingoActive = false;
				boardInfoBox.Text = "Uh oh, not enough goals match the criteria set by the board settings, and a board was unable to be generated.";
				goalInfoBox.Text = boardInfoBox.Text;
				foreach (Label l in labels) {
					l.Text = "";
				}
				return;
			}
			#endregion

			// I should make all these regions separate functions so it'll look better
			//Shuffle the tiles 1-25 in a random order
			List<int> bingoBoardTiles = new List<int>(Enumerable.Range(0, 24));
			Random bingoBoardRandomizer = new Random(seed);
			List<int> bingoBoardShuffledTiles = new List<int>();

			while (bingoBoardTiles.Count > 0) {
				int bingoTile = bingoBoardRandomizer.Next(bingoBoardTiles.Count);
				bingoBoardShuffledTiles.Add(bingoBoardTiles[bingoTile]);
				bingoBoardTiles.RemoveAt(bingoTile);
			}

			// This will select a random square from a shuffled list, and will mark it difficulty Very Hard, until there's enough of those.
			// Then it'll do hard, then medium, and the unprocessed one are easy by default.
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

			// Now it's time to actually put goals. It checks every tile in order, looks at its set difficulty, and picks a random goal accordingly.
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

		// Write to the config XML that this is the last game the player played (is this needed?)
		private void SaveLastGameData() {
			XmlDocument doc = new XmlDocument();
			doc.Load(@"BingoConfig.xml");
			XmlNode node = doc.DocumentElement.SelectSingleNode("lastgame");
			node.InnerText = comboBox1.SelectedItem.ToString();
			doc.Save(@"BingoConfig.xml");
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
					if (bingoActive && theBingoBoard[selectedLabel].Attributes["description"] != null) {
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
					if (bingoActive && theBingoBoard[selectedLabel].Attributes["description"] != null) {
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

		private void resetBoardColors() {
			foreach (Label l in labels) {
				l.BackColor = colors[(0)];
			}
			if (selectedLabel != 25) {
				Point locationToPutA = new Point(labels[selectedLabel].Location.X + 5, labels[selectedLabel].Location.Y + 5);
				labels[selectedLabel].Location = locationToPutA;
				Font fontToPutA = new Font(labels[selectedLabel].Font, FontStyle.Regular);
				labels[selectedLabel].Font = fontToPutA;
				Size sizeToPutA = new Size(labels[selectedLabel].Size.Height - 10, labels[selectedLabel].Size.Width - 10);
				labels[selectedLabel].Size = sizeToPutA;
			}
			selectedLabel = 25;
		}
		#endregion

		// Reads for global hotkey presses.
		protected override void WndProc(ref Message keyPressed) {
			if (keyPressed.Msg == 0x0312) {
				int key = keyPressed.WParam.ToInt32();
				if (key == 7) {		// Toggle hotkeys
					checkBox2.Checked = !checkBox2.Checked;
				}
				else if (!unhideButton.Visible) {
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
				else if (key == 6) {		// Unhide board
					hideBoard(false);
				}

			}
			base.WndProc(ref keyPressed);
		}

		#region clicking on stuff on the form etc
		private void button1_Click(object sender, EventArgs e) {
			hideBoard(checkBox1.Checked);
			SaveLastGameData();
			GenerateNewSheet();
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
			hotkeys.UnregisterHotkeys(true);
			SaveLastGameData();
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
			difficulty = trackBar1.Value * Convert.ToInt32(trackBar1.Enabled);
			generateUID();
		}

		private void trackBar2_Scroll(object sender, EventArgs e) {
			length = trackBar2.Value * Convert.ToInt32(trackBar2.Enabled);
			generateUID();
		}

		private void checkBoxLength_CheckedChanged(object sender, EventArgs e) {
			trackBar2.Enabled = !checkBoxLength.Checked;
			trackBar2_Scroll(sender, e);
			generateUID();
		}

		private void checkBoxDiff_CheckedChanged(object sender, EventArgs e) {
			trackBar1.Enabled = !checkBoxDiff.Checked;
			trackBar1_Scroll(sender, e);
			generateUID();
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e) {
			if (!checkBox2.Checked) {
				hotkeys.UnregisterHotkeys(false);
				labelHKNotifier.Text = "Hotkeys DISABLED";
			}
			else {
				hotkeys.RegisterHotkeys((uint)comboU.SelectedValue, (uint)comboD.SelectedValue, (uint)comboL.SelectedValue, (uint)comboR.SelectedValue, (uint)comboP.SelectedValue, (uint)comboN.SelectedValue, (uint)comboH.SelectedValue, (uint)comboT.SelectedValue, modifierU, modifierD, modifierL, modifierR, modifierP, modifierN, modifierH, modifierT);
				labelHKNotifier.Text = "";
			}
		}
		#endregion

		#region modifiercheckboxes
		private void ctrlU_CheckedChanged(object sender, EventArgs e) {
			modifierU = Convert.ToUInt32(ctrlU.Checked) * 2 + Convert.ToUInt32(altU.Checked) + Convert.ToUInt32(shiftU.Checked) * 4 + Convert.ToUInt32(winU.Checked) * 8;
		}

		private void ctrlD_CheckedChanged(object sender, EventArgs e) {
			modifierD = Convert.ToUInt32(ctrlD.Checked) * 2 + Convert.ToUInt32(altD.Checked) + Convert.ToUInt32(shiftD.Checked) * 4 + Convert.ToUInt32(winD.Checked) * 8;
		}

		private void ctrlL_CheckedChanged(object sender, EventArgs e) {
			modifierL = Convert.ToUInt32(ctrlL.Checked) * 2 + Convert.ToUInt32(altL.Checked) + Convert.ToUInt32(shiftL.Checked) * 4 + Convert.ToUInt32(winL.Checked) * 8;
		}

		private void ctrlR_CheckedChanged(object sender, EventArgs e) {
			modifierR = Convert.ToUInt32(ctrlR.Checked) * 2 + Convert.ToUInt32(altR.Checked) + Convert.ToUInt32(shiftR.Checked) * 4 + Convert.ToUInt32(winR.Checked) * 8;
		}

		private void ctrlP_CheckedChanged(object sender, EventArgs e) {
			modifierP = Convert.ToUInt32(ctrlP.Checked) * 2 + Convert.ToUInt32(altP.Checked) + Convert.ToUInt32(shiftP.Checked) * 4 + Convert.ToUInt32(winP.Checked) * 8;
		}

		private void ctrlN_CheckedChanged(object sender, EventArgs e) {
			modifierN = Convert.ToUInt32(ctrlN.Checked) * 2 + Convert.ToUInt32(altN.Checked) + Convert.ToUInt32(shiftN.Checked) * 4 + Convert.ToUInt32(winN.Checked) * 8;
		}

		private void ctrlH_CheckedChanged(object sender, EventArgs e) {
			modifierH = Convert.ToUInt32(ctrlH.Checked) * 2 + Convert.ToUInt32(altH.Checked) + Convert.ToUInt32(shiftH.Checked) * 4 + Convert.ToUInt32(winH.Checked) * 8;
		}

		private void ctrlT_CheckedChanged(object sender, EventArgs e) {
			modifierT = Convert.ToUInt32(ctrlT.Checked) * 2 + Convert.ToUInt32(altT.Checked) + Convert.ToUInt32(shiftT.Checked) * 4 + Convert.ToUInt32(winT.Checked) * 8;
		}
		#endregion

		// Upon clicking the apply button, make the hotkeys work, and update the config file.
		private void button2_Click(object sender, EventArgs e) {
			hotkeys.UnregisterHotkeys(true);
			hotkeys.RegisterHotkeys((uint)comboU.SelectedValue, (uint)comboD.SelectedValue, (uint)comboL.SelectedValue, (uint)comboR.SelectedValue, (uint)comboP.SelectedValue, (uint)comboN.SelectedValue, (uint)comboH.SelectedValue, (uint)comboT.SelectedValue, modifierU, modifierD, modifierL, modifierR, modifierP, modifierN, modifierH, modifierT);

			XmlDocument doc = new XmlDocument();
			doc.Load(@"BingoConfig.xml");
			XmlNode node = doc.DocumentElement.SelectSingleNode("hotkeys");
			node.Attributes["U"].InnerText = comboU.SelectedValue.ToString();
			node.Attributes["D"].InnerText = comboD.SelectedValue.ToString();
			node.Attributes["L"].InnerText = comboL.SelectedValue.ToString();
			node.Attributes["R"].InnerText = comboR.SelectedValue.ToString();
			node.Attributes["P"].InnerText = comboP.SelectedValue.ToString();
			node.Attributes["N"].InnerText = comboN.SelectedValue.ToString();
			node.Attributes["H"].InnerText = comboH.SelectedValue.ToString();
			node.Attributes["T"].InnerText = comboT.SelectedValue.ToString();
			XmlNode node2 = doc.DocumentElement.SelectSingleNode("modifiers");
			node.Attributes["U"].InnerText = (Convert.ToUInt32(ctrlU.Checked) * 2 + Convert.ToUInt32(altU.Checked) + Convert.ToUInt32(shiftU.Checked) * 4 + Convert.ToUInt32(winU.Checked) * 8).ToString();
			node.Attributes["D"].InnerText = (Convert.ToUInt32(ctrlD.Checked) * 2 + Convert.ToUInt32(altD.Checked) + Convert.ToUInt32(shiftD.Checked) * 4 + Convert.ToUInt32(winD.Checked) * 8).ToString();
			node.Attributes["L"].InnerText = (Convert.ToUInt32(ctrlL.Checked) * 2 + Convert.ToUInt32(altL.Checked) + Convert.ToUInt32(shiftL.Checked) * 4 + Convert.ToUInt32(winL.Checked) * 8).ToString();
			node.Attributes["R"].InnerText = (Convert.ToUInt32(ctrlR.Checked) * 2 + Convert.ToUInt32(altR.Checked) + Convert.ToUInt32(shiftR.Checked) * 4 + Convert.ToUInt32(winR.Checked) * 8).ToString();
			node.Attributes["P"].InnerText = (Convert.ToUInt32(ctrlP.Checked) * 2 + Convert.ToUInt32(altP.Checked) + Convert.ToUInt32(shiftP.Checked) * 4 + Convert.ToUInt32(winP.Checked) * 8).ToString();
			node.Attributes["N"].InnerText = (Convert.ToUInt32(ctrlN.Checked) * 2 + Convert.ToUInt32(altN.Checked) + Convert.ToUInt32(shiftN.Checked) * 4 + Convert.ToUInt32(winN.Checked) * 8).ToString();
			node.Attributes["H"].InnerText = (Convert.ToUInt32(ctrlH.Checked) * 2 + Convert.ToUInt32(altH.Checked) + Convert.ToUInt32(shiftH.Checked) * 4 + Convert.ToUInt32(winH.Checked) * 8).ToString();
			node.Attributes["T"].InnerText = (Convert.ToUInt32(ctrlT.Checked) * 2 + Convert.ToUInt32(altT.Checked) + Convert.ToUInt32(shiftT.Checked) * 4 + Convert.ToUInt32(winT.Checked) * 8).ToString();
			doc.Save(@"BingoConfig.xml");
		}
	}
}