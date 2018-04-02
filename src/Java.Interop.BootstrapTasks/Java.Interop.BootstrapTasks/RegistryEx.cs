﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Java.Interop.BootstrapTasks
{
	internal static class RegistryEx
	{
		const string ADVAPI = "advapi32.dll";

		public static UIntPtr CurrentUser = (UIntPtr)0x80000001;
		public static UIntPtr LocalMachine = (UIntPtr)0x80000002;

		[DllImport (ADVAPI, CharSet = CharSet.Unicode, SetLastError = true)]
		static extern int RegOpenKeyEx (UIntPtr hKey, string subKey, uint reserved, uint sam, out UIntPtr phkResult);

		[DllImport (ADVAPI, CharSet = CharSet.Unicode, SetLastError = true)]
		static extern int RegQueryValueExW (UIntPtr hKey, string lpValueName, int lpReserved, out uint lpType,
			StringBuilder lpData, ref uint lpcbData);

		[DllImport (ADVAPI, CharSet = CharSet.Unicode, SetLastError = true)]
		static extern int RegSetValueExW (UIntPtr hKey, string lpValueName, int lpReserved,
			uint dwType, string data, uint cbData);

		[DllImport (ADVAPI, CharSet = CharSet.Unicode, SetLastError = true)]
		static extern int RegSetValueExW (UIntPtr hKey, string lpValueName, int lpReserved,
			uint dwType, IntPtr data, uint cbData);

		[DllImport (ADVAPI, CharSet = CharSet.Unicode, SetLastError = true)]
		static extern int RegCreateKeyEx (UIntPtr hKey, string subKey, uint reserved, string @class, uint options,
			uint samDesired, IntPtr lpSecurityAttributes, out UIntPtr phkResult, out Disposition lpdwDisposition);

		[DllImport ("advapi32.dll", SetLastError = true)]
		static extern int RegCloseKey (UIntPtr hKey);

		public static string GetValueString (UIntPtr key, string subkey, string valueName, Wow64 wow64)
		{
			UIntPtr regKeyHandle;
			uint sam = (uint)Rights.QueryValue + (uint)wow64;
			if (RegOpenKeyEx (key, subkey, 0, sam, out regKeyHandle) != 0)
				return null;

			try {
				uint type;
				var sb = new StringBuilder (2048);
				uint cbData = (uint)sb.Capacity;
				if (RegQueryValueExW (regKeyHandle, valueName, 0, out type, sb, ref cbData) == 0) {
					return sb.ToString ();
				}
				return null;
			} finally {
				RegCloseKey (regKeyHandle);
			}
		}

		public static void SetValueString (UIntPtr key, string subkey, string valueName, string value, Wow64 wow64)
		{
			UIntPtr regKeyHandle;
			uint sam = (uint)(Rights.CreateSubKey | Rights.SetValue) + (uint)wow64;
			uint options = (uint)Options.NonVolatile;
			Disposition disposition;
			if (RegCreateKeyEx (key, subkey, 0, null, options, sam, IntPtr.Zero, out regKeyHandle, out disposition) != 0) {
				throw new Exception ("Could not open or craete key");
			}

			try {
				uint type = (uint)ValueType.String;
				uint lenBytesPlusNull = ((uint)value.Length + 1) * 2;
				var result = RegSetValueExW (regKeyHandle, valueName, 0, type, value, lenBytesPlusNull);
				if (result != 0)
					throw new Exception (string.Format ("Error {0} setting registry key '{1}{2}@{3}'='{4}'",
						result, key, subkey, valueName, value));
			} finally {
				RegCloseKey (regKeyHandle);
			}
		}

		[Flags]
		enum Rights : uint
		{
			None = 0,
			QueryValue = 0x0001,
			SetValue = 0x0002,
			CreateSubKey = 0x0004,
			EnumerateSubKey = 0x0008,
		}

		enum Options
		{
			BackupRestore = 0x00000004,
			CreateLink = 0x00000002,
			NonVolatile = 0x00000000,
			Volatile = 0x00000001,
		}

		public enum Wow64 : uint
		{
			Key64 = 0x0100,
			Key32 = 0x0200,
		}

		enum ValueType : uint
		{
			None = 0, //REG_NONE
			String = 1, //REG_SZ
			UnexpandedString = 2, //REG_EXPAND_SZ
			Binary = 3, //REG_BINARY
			DWord = 4, //REG_DWORD
			DWordLittleEndian = 4, //REG_DWORD_LITTLE_ENDIAN
			DWordBigEndian = 5, //REG_DWORD_BIG_ENDIAN
			Link = 6, //REG_LINK
			MultiString = 7, //REG_MULTI_SZ
			ResourceList = 8, //REG_RESOURCE_LIST
			FullResourceDescriptor = 9, //REG_FULL_RESOURCE_DESCRIPTOR
			ResourceRequirementsList = 10, //REG_RESOURCE_REQUIREMENTS_LIST
			QWord = 11, //REG_QWORD
			QWordLittleEndian = 11, //REG_QWORD_LITTLE_ENDIAN
		}

		enum Disposition : uint
		{
			CreatedNewKey = 0x00000001,
			OpenedExistingKey = 0x00000002,
		}
	}
}
