﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using RGiesecke.DllExport;
using Hackery;

namespace cgamex86 {
	class cgamex86 {
		[DllExport("dllEntry", CallingConvention = CallingConvention.Cdecl)]
		public static void dllEntry(ReturnInfo RetInfo, DispatchTable SyscallTable, IntPtr Unknown) {
			SDK.CGameInit(SyscallTable);
			SDK.PRINT(Colors.Red + "[CGame] Hello Quake Live!\n");

			IntPtr _uix86 = Kernel32.LoadLibrary("cliq3\\orig\\_cgamex86.dll");
			dllEntryFunc _dllEntry = Kernel32.GetProcAddress<dllEntryFunc>(_uix86, "dllEntry");
			_dllEntry(RetInfo, SyscallTable, Unknown);
		}
	}
}