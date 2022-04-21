using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSSystem.IO
{
	public class ZipFile
	{
		public static void Compress(string folderPath, string zipFilePath, Encoding encoding = default)
		{
			try
			{
				if (!Directory.Exists(folderPath)) return;
				if (File.Exists(zipFilePath)) File.Delete(zipFilePath);
				if (encoding == null) encoding = Encoding.UTF8;
				System.IO.Compression.ZipFile.CreateFromDirectory(folderPath, zipFilePath, CompressionLevel.Fastest, true, encoding);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static void CompressV2(string folderPath, string zipFilePath, Encoding encoding = default)
		{
			try
			{
				if (!Directory.Exists(folderPath)) return;
				if (File.Exists(zipFilePath)) File.Delete(zipFilePath);
				if (encoding == null)
				{
					encoding = Encoding.UTF8;
				}
				using (var zipArchive = ICSharpCode.SharpZipLib.Zip.ZipFile.Create(zipFilePath))
				{
					zipArchive.UseZip64 = UseZip64.On;



					zipArchive.Close();
				}

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static List<string> GetFileEntryPaths(string zipFilePath, params string[] fileExtensions)
		{
			List<string> fileEntries = new List<string>();
			try
			{
				using (var zipStream = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{

					using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read, true))
					{
						if (zipArchive.Entries?.Count > 0)
						{
							foreach (ZipArchiveEntry entry in zipArchive.Entries)
							{
								if (entry.FullName.EndsWith("/") || entry.FullName.EndsWith("\\")) // Folder
								{
									//folderEntries.Add(entry.FullName);
								}
								else
								{
									var fileExt = Path.GetExtension(entry.FullName);
									if (fileExtensions == null || fileExtensions.Length == 0 || fileExtensions.Contains(fileExt, StringComparer.InvariantCultureIgnoreCase))
									{
										fileEntries.Add(entry.FullName);
									}
								}
							}

						}
						zipArchive.Dispose();
					}
					zipStream.Close();
					zipStream.Dispose();
				}
			}
			catch
			{
			}
			return fileEntries;
		}
		public static List<string> GetFolderEntryPaths(string zipFilePath)
		{
			List<string> folderEntries = new List<string>();
			try
			{
				using (Stream zipStream = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read, true))
					{
						if (zipArchive.Entries?.Count > 0)
						{
							foreach (ZipArchiveEntry entry in zipArchive.Entries)
							{
								if (entry.FullName.EndsWith("/") || entry.FullName.EndsWith("\\")) // Folder
								{
									folderEntries.Add(entry.FullName);
								}
							}

						}
						zipArchive.Dispose();
					}
					zipStream.Close();
					zipStream.Dispose();
				}
			}
			catch
			{
			}
			return folderEntries;
		}

		public static Task<byte[]> GetEntryBytesAsync(string zipFilePath, string entryPath)
		{
			return GetEntryBytesAsync(new FileInfo(zipFilePath), entryPath);
		}
		public static async Task<byte[]> GetEntryBytesAsync(FileInfo zipFile, string entryPath)
		{
			byte[] result = null;
			try
			{
				if (zipFile?.Exists ?? false)
				{
					using (Stream zipStream = zipFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						try
						{
							result = await _GetEntryBytesAsync(zipStream, entryPath);
						}
						catch { }
						zipStream.Close();
						zipStream.Dispose();
					}
				}

			}
			catch { }
			return result;
		}

		static async Task<byte[]> _GetEntryBytesAsync(Stream stream, string entryPath)
		{
			byte[] result = null;
			try
			{
				using (var zipArchive = new ICSharpCode.SharpZipLib.Zip.ZipFile(stream))
				{
					if (stream.Length > int.MaxValue)
					{
						zipArchive.UseZip64 = UseZip64.On;
					}
					try
					{
						var entry = zipArchive.GetEntry(entryPath);
						if (entry != null)
						{
							using (Stream inStream = zipArchive.GetInputStream(entry))
							{
								try
								{
									using (MemoryStream outStream = new MemoryStream())
									{
										try
										{
											await inStream.CopyToAsync(outStream, (int)entry.Size);
										}
										catch { }
										outStream.Close();
										outStream.Dispose();
										result = outStream.ToArray();
									}
								}
								catch { }
								inStream.Close();
								inStream.Dispose();
							}
						}
					}
					catch { }
					zipArchive.Close();
				}
			}
			catch { }
			return result;
		}

		static Encoding GetEncoding(string filename)
		{
			var bom = new byte[4];
			using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				file.Read(bom, 0, 4);
				file.Close();
			}

			if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
			if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
			if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
			if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
			if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
			return Encoding.ASCII;
		}
	}
}
