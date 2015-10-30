﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void dllEntryFunc(ReturnInfo RetInfo, DispatchTable SyscallTable, IntPtr Unknown);

[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
public delegate void PrintFunc(string Str);

public unsafe struct DispatchTable {
	static HashSet<Delegate> Delegates = new HashSet<Delegate>();
	public IntPtr DispatchTablePtr;

	public static readonly DispatchTable Zero = new DispatchTable(IntPtr.Zero);

	public DispatchTable(IntPtr DispatchTablePtr) {
		this.DispatchTablePtr = DispatchTablePtr;
	}

	public IntPtr this[int Syscall] {
		get {
			return ((IntPtr*)DispatchTablePtr.ToPointer())[Syscall];
		}
		set {
			((IntPtr*)DispatchTablePtr.ToPointer())[Syscall] = value;
		}
	}

	public T GetFunc<T>(int Syscall) where T : class {
		if (!typeof(Delegate).IsAssignableFrom(typeof(T)))
			throw new Exception("Type has to be a delegate");
		return Marshal.GetDelegateForFunctionPointer(this[Syscall], typeof(T)) as T;
	}

	public void SetFunc(int Syscall, Delegate D) {
		if (!Delegates.Contains(D))
			Delegates.Add(D);
		this[Syscall] = Marshal.GetFunctionPointerForDelegate(D);
	}

	public override bool Equals(object Obj) {
		if (Obj == null || GetType() != Obj.GetType())
			return false;

		return ((DispatchTable)Obj).DispatchTablePtr.Equals(DispatchTablePtr);
	}

	public override int GetHashCode() {
		return DispatchTablePtr.GetHashCode();
	}

	public static bool operator ==(DispatchTable A, DispatchTable B) {
		return A.Equals(B);
	}

	public static bool operator !=(DispatchTable A, DispatchTable B) {
		return !(A == B);
	}
}

public unsafe struct ReturnInfo {
	public IntPtr ReturnInfoPtr;

	public ReturnInfo(int Size) {
		ReturnInfoPtr = IntPtr.Zero;
		AllocateDispatchTable(Size);
	}

	public DispatchTable GetDispatchTable() {
		return *(DispatchTable*)ReturnInfoPtr.ToPointer();
	}

	public void AllocateDispatchTable(int Size = 1) {
		/*if (*(IntPtr*)ReturnInfoPtr.ToPointer() != IntPtr.Zero)
			Marshal.FreeHGlobal(*(IntPtr*)ReturnInfoPtr.ToPointer());*/

		(*(IntPtr*)ReturnInfoPtr.ToPointer()) = Marshal.AllocHGlobal(Size * sizeof(int));
	}
}

public static class Colors {
	public const string Red = "^1";
	public const string Green = "^2";
	public const string Yellow = "^3";
	public const string Blue = "^4";
	public const string Cyan = "^5";
	public const string Magenta = "^6";
	public const string White = "^7";
}

public static partial class SDK {
	static DispatchTable QAGameTable, CGameTable, UITable;

	public static void QAGameInit(DispatchTable SyscallTable) {
		Console.WriteLine("QAGameTable: {0}", SyscallTable.DispatchTablePtr);
		if (QAGameTable != DispatchTable.Zero)
			return;
		QAGameTable = SyscallTable;
	}

	public static void CGameInit(DispatchTable SyscallTable) {
		Console.WriteLine("CGameTable: {0}", SyscallTable.DispatchTablePtr);
		if (CGameTable != DispatchTable.Zero)
			return;
		CGameTable = SyscallTable;
	}

	public static void UIInit(DispatchTable SyscallTable) {
		Console.WriteLine("UITable: {0}", SyscallTable.DispatchTablePtr);
		if (UITable != DispatchTable.Zero)
			return;
		UITable = SyscallTable;
	}

	public static string SanitizeString(string Str) {
		return Str.Replace("^1", "").Replace("^2", "")
			.Replace("^3", "").Replace("^4", "")
			.Replace("^5", "").Replace("^6", "")
			.Replace("^7", "").Trim();
	}

	public static void PRINT(string Fmt, params string[] Args) {
		if (Args != null && Args.Length > 0)
			Fmt = string.Format(Fmt, Args);
		Console.WriteLine("{0}", SanitizeString(Fmt));
		if (UITable == DispatchTable.Zero)
			return;
		UITable.GetFunc<PrintFunc>(0)(Fmt);
	}
}