﻿using System;

/// <summary>
/// Provides memory management convenience functions.
/// </summary>
static class MemoryManagement {
	/// <summary>
	/// <para>A handle to safely temporarily change memory protection.</para>
	/// <para>The protection is reset when the handle is disposed.</para>
	/// </summary>
	public class VirtualProtectHandle : IDisposable {
		/// <summary>
		/// The address of the first byte that had its protection changed.
		/// </summary>
		public IntPtr Address {
			get;
			private set;
		}

		/// <summary>
		/// The size of the memory range with changed protection.
		/// </summary>
		public int Size {
			get;
			private set;
		}

		/// <summary>
		/// The previous protection setting.
		/// </summary>
		public MemProtection OldProtection {
			get;
			private set;
		}

		/// <summary>
		/// Creates a new <see cref="VirtualProtectHandle"/> instance with the given parameters.
		/// </summary>
		/// <param name="address">The address of the first byte that had its protection changed.</param>
		/// <param name="size">The size of the memory range with changed protection.</param>
		/// <param name="oldProtection">The previous protection setting.</param>
		public VirtualProtectHandle(IntPtr address, int size, MemProtection oldProtection) {
			Address = address;
			Size = size;
			OldProtection = oldProtection;
		}

		/// <summary>
		/// Resets the memory protection to its previous value.
		/// </summary>
		public void Dispose() {
			Native.VirtualProtect(Address, Size, OldProtection);
		}
	}

	public static VirtualProtectHandle Protect(IntPtr address, int size, MemProtection newProtection) {
		MemProtection oldProtection;
		Native.VirtualProtect(address, size, newProtection, out oldProtection);
		return new VirtualProtectHandle(address, size, oldProtection);
	}
}