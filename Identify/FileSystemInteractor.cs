using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Identify
{
	public class FileSystemInteractor
	{
		public struct BY_HANDLE_FILE_INFORMATION
		{
			public uint FileAttributes;
			public FILETIME CreationTime;
			public FILETIME LastAccessTime;
			public FILETIME LastWriteTime;
			public uint VolumeSerialNumber;
			public uint FileSizeHigh;
			public uint FileSizeLow;
			public uint NumberOfLinks;
			public uint FileIndexHigh;
			public uint FileIndexLow;
		}

		public struct IO_STATUS_BLOCK
		{
			uint status;
			ulong information;
		}

		public struct _FILE_INTERNAL_INFORMATION
		{
			public ulong IndexNumber;
		}

		// Abbreviated, there are more values than shown
		public enum FILE_INFORMATION_CLASS
		{
			FileDirectoryInformation = 1,   // 1
			FileFullDirectoryInformation,   // 2
			FileBothDirectoryInformation,   // 3
			FileBasicInformation,           // 4
			FileStandardInformation,        // 5
			FileInternalInformation         // 6
		}

		[DllImport("ntdll.dll", SetLastError = true)]
		public static extern IntPtr NtQueryInformationFile(
			IntPtr fileHandle,
			ref IO_STATUS_BLOCK IoStatusBlock,
			IntPtr pInfoBlock,
			uint length,
			FILE_INFORMATION_CLASS fileInformation);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool GetFileInformationByHandle(
			IntPtr hFile,
			out BY_HANDLE_FILE_INFORMATION lpFileInformation);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern SafeFileHandle CreateFile(
			string lpFileName,
			[MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess,
			[MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
			IntPtr securityAttributes,
			[MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
			uint dwFlagsAndAttributes,
			IntPtr hTemplateFile
		);

		public static FileIdentityInformation GetVolumeIdAndUniqueIdForFile(string fullPathWithFileName)
		{
			var fileHandleInformation = new FileSystemInteractor.BY_HANDLE_FILE_INFORMATION();
			var fileInformation = new FileInfo(fullPathWithFileName);

			try
			{
				var fileStream = fileInformation.Open(FileMode.Open, FileAccess.Read);

				// The "Dangerous" aspect here refers to the 'SetHandleAsInvalid'. This allows the handle to be re-used
				// and, potentially, to point to another resource. This can pose a security risk should the attempt be 
				// made by external code to retrieve objects on the file system, itself, using this newly invalid IntPtr 
				// as it could point to another resource, entirely. This does not appear to be an issue for Sync as we 
				// do not expose this information.  
				FileSystemInteractor.GetFileInformationByHandle(fileStream.SafeFileHandle.DangerousGetHandle(), out fileHandleInformation);

				fileStream.Close();

				// The cast is very important. If you omit the cast, you'll be shifting a 32 bit value 32 bits to 
				// the left. Shift operators on 32 bit variables will use shift stuff by right-hand-side mod 32. 
				// Effectively, shifting a uint 32 bits to the left is a no-op. Casting to ulong prevents this.
				return
					new FileIdentityInformation
					(
						fileHandleInformation.VolumeSerialNumber,
						fileHandleInformation.FileIndexHigh,
						fileHandleInformation.FileIndexLow

					);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static FileIdentityInformation GetVolumeIdAndUniqueIdForFolder(string fullPathWithFolderName)
		{
			var directoryHandleInformation = new FileSystemInteractor.BY_HANDLE_FILE_INFORMATION();

			try
			{
				// OPEN_EXISTING (FileMode.Open) is specified as we know the folder exists when passed into this method;
				// however, if it doesn't, a FileNotFoundException is thrown
				var createdFolder =
					FileSystemInteractor.CreateFile(
						fullPathWithFolderName,
						FileAccess.Read,
						FileShare.Read,
						IntPtr.Zero,
						FileMode.Open,
						Kernel32.FILE_FLAG_BACKUP_SEMANTICS,
						IntPtr.Zero
					);

				FileSystemInteractor.GetFileInformationByHandle(createdFolder.DangerousGetHandle(), out directoryHandleInformation);


				return
					new FileIdentityInformation
					(
						directoryHandleInformation.VolumeSerialNumber,
						directoryHandleInformation.FileIndexHigh,
						directoryHandleInformation.FileIndexLow
					);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}

	/// <summary>
	/// The identifier (low and high parts) and the volume serial number uniquely identify a file on 
	/// a single computer. To determine whether two open handles represent the same file, combine 
	/// the identifier and the volume serial number for each file and compare them.
	/// </summary>
	public class FileIdentityInformation
	{
		/// <summary>
		/// The serial number of the volume that contains a file.
		/// </summary>
		public UInt64 VolumeId { get; set; }

		/// <summary>
		/// The high-order part of a unique identifier that is associated with a file prepended to 
		/// the low-order part of a unique identifier that is associated with a file.
		/// </summary>
		public UInt64 UniqueId => (ulong)FileIndexHigh << 32 | FileIndexLow;

		public UInt32 FileIndexHigh { get; set; }

		public UInt32 FileIndexLow { get; set; }

		public FileIdentityInformation(UInt64 volumeId, UInt32 fileHighIndex, UInt32 fileLowIndex)
		{
			this.VolumeId = volumeId;
			this.FileIndexHigh = fileHighIndex;
			this.FileIndexLow = fileLowIndex;
		}
	}
}