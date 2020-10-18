using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AlternateStreams
{
	// Based on https://www.dreamincode.net/forums/topic/90666-reading-and-writing-alternate-streams-in-c%23/
	// NTFS only!
	public static class AlternateStreamsApi
    {
		#region Constants

		// Constants used by the Win32 api functions.  They can be found in the documentation and header files.
		public const UInt32 GENERIC_READ = 0x80000000;
		public const UInt32 GENERIC_WRITE = 0x40000000;
		public const UInt32 FILE_SHARE_READ = 0x00000001;
		public const UInt32 FILE_SHARE_WRITE = 0x00000002;
		public const UInt32 FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

		public const UInt32 CREATE_NEW = 1;
		public const UInt32 CREATE_ALWAYS = 2;
		public const UInt32 OPEN_EXISTING = 3;
		public const UInt32 OPEN_ALWAYS = 4;
		public const UInt32 TRUNCATE_EXISTING = 5;

		#endregion

		#region Dll imports

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteFile(string fileName);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ReadFile(IntPtr hFile, IntPtr lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool WriteFile(IntPtr hFile, IntPtr bytes, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, int overlapped);

		[DllImport("kernel32.dll", EntryPoint="RtlZeroMemory")]
		public static extern void ZeroMemory(IntPtr ptr, int size);

		#endregion

		#region Public methods
		
		public static void WriteAlternateStream(string currentFile, string altStreamName, string text)
		{
            string altStream = $"{currentFile}:{altStreamName}";
			IntPtr txtBuffer = IntPtr.Zero;
			IntPtr hFile = IntPtr.Zero;
			DeleteFile(altStream);
			try
			{
				hFile = CreateFile(altStream, GENERIC_WRITE, 0, IntPtr.Zero, CREATE_ALWAYS, 0, IntPtr.Zero);
				if (-1 != hFile.ToInt32())  // check the return code for success
				{
					txtBuffer = Marshal.StringToHGlobalUni(text);
                    var nBytes = (uint)text.Length;
					bool bRtn = WriteFile(hFile, txtBuffer, sizeof(char) * nBytes, out var count, 0);
					if (!bRtn)
                    {
                        if ((sizeof(char) * nBytes) != count)
							throw new Exception($"Bytes written {count} should be {sizeof(char) * nBytes} for file {altStream}.");
                        throw new Exception("WriteFile() returned false");
                    }
				}
				else throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			catch (Exception exception)
			{
				var msg = $"Exception caught in WriteAlternateStream()\n  '{exception.Message}'\n  for file '{altStream}'.";
				Debug.WriteLine(msg);
			}
			finally
			{
				CloseHandle(hFile);
				hFile = IntPtr.Zero;
				Marshal.FreeHGlobal(txtBuffer);
				GC.Collect();
			}
		}

		public static string ReadAlternateStream(string currentFile, string altStreamName)
		{
			string returnString = string.Empty;
			string altStream = $"{currentFile}:{altStreamName}";
            IntPtr hFile = IntPtr.Zero;
            IntPtr buffer = IntPtr.Zero;
			try
			{
				hFile = CreateFile(altStream, GENERIC_READ, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
				if (-1 != hFile.ToInt32())
				{
					buffer = Marshal.AllocHGlobal(1000 * sizeof(char));
					ZeroMemory(buffer, 1000 * sizeof(char));
                    bool bRtn = ReadFile(hFile, buffer, 1000 * sizeof(char), out var nBytes, IntPtr.Zero);
					if (bRtn)
					{
						if (nBytes > 0)
							returnString = Marshal.PtrToStringAuto(buffer);
						else throw new Exception("ReadFile() returned true but read zero bytes");
					}
					else
					{
						if (nBytes <= 0)
							throw new Exception("ReadFile() read zero bytes.");
						else
							throw new Exception("ReadFile() returned false");
					}
				}
				else
				{
					Exception exception = new Win32Exception(Marshal.GetLastWin32Error());
					if (!exception.Message.Contains("cannot find the file"))
						throw exception;
				}
			}
			catch (Exception exception)
            {
                string msg = $"Exception caught in ReadAlternateStream(), '{exception.Message}'\n  for file '{currentFile}'.";
				Debug.WriteLine(msg);
            }
			finally
			{
				CloseHandle(hFile);
				hFile = IntPtr.Zero;
				if (buffer != IntPtr.Zero)
					Marshal.FreeHGlobal(buffer);
				GC.Collect();
			}
			return returnString;
		}
		#endregion
	}
}