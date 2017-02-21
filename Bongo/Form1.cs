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

namespace Bongo {
    public partial class Form1 : Form {

        Label[] labels = new Label[25];
        int[] sheetLayout;

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            //https://github.com/Joshimuz/mcbingo/blob/master/bingo.js





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

            // Determine the board layout
            if (uniqueCode % 1000000 / 100000 == 1) {
                // fixed bingo layout
                sheetLayout = new int[] { 1, 2, 0, 2, 1,
                                          2, 0, 1, 0, 2,
                                          0, 1, 3, 1, 0,
                                          2, 0, 1, 0, 2,
                                          1, 2, 0, 2, 1 };
            }
            else {
                // random bingo layout
                sheetLayout = new int[25];
            }

            for (int i = 0; i < 25; i++) {
                labels[i].Text = sheetLayout[i].ToString();
            }

            // Determine the difficulty
            int difficulty = uniqueCode % 10000000 / 1000000;
            switch (difficulty) {
                case 1:
                    // Easiest
                    break;
                case 2:
                    // Easy
                    break;
                case 3:
                    // Moderate
                    break;
                case 4:
                    // Hard
                    break;
                case 5:
                    // Hardest
                    break;
                default:
                    // Moderate (default)
                    break;
            }

        }

        private void button1_Click(object sender, EventArgs e) {
            generateNewSheet(int.Parse(seedDisplayBox.Text));
        }
    }
}
