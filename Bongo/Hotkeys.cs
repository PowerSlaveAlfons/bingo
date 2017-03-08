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
	class Hotkeys {

		IntPtr _hWnd;

		[DllImport("user32.dll")]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

		[DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		public Hotkeys(IntPtr hWnd) {
			_hWnd = hWnd;
		}

		public void RegisterHotkeys() {
			RegisterHotKey(_hWnd, 0, 2, (uint)Keys.I); //up
			RegisterHotKey(_hWnd, 1, 2, (uint)Keys.K); //down
			RegisterHotKey(_hWnd, 2, 2, (uint)Keys.J); //left
			RegisterHotKey(_hWnd, 3, 2, (uint)Keys.L); //right
			RegisterHotKey(_hWnd, 4, 2, (uint)Keys.U); //previous color
			RegisterHotKey(_hWnd, 5, 2, (uint)Keys.O); //next color
			RegisterHotKey(_hWnd, 6, 2, (uint)Keys.M); //special action
		}

		public void UnregisterHotkeys() {
			UnregisterHotKey(_hWnd, 0);
			UnregisterHotKey(_hWnd, 1);
			UnregisterHotKey(_hWnd, 2);
			UnregisterHotKey(_hWnd, 3);
			UnregisterHotKey(_hWnd, 4);
			UnregisterHotKey(_hWnd, 5);
			UnregisterHotKey(_hWnd, 6);
		}

	}
}
