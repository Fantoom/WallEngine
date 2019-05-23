using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using System.Diagnostics;

namespace WPEngine.WPEngineClasses
{
	class ShareController
	{
		
		public static void SaveAsZip(Project project, string path = null , bool ShowFolder = true)
		{
			if (project == null)
			{
				return;
			}
			if (path == null)
			{
				SaveFileDialog Fdialog = new SaveFileDialog();
				Fdialog.Filter = "Zip (*.zip)|*.zip";
				Fdialog.DefaultExt = "zip";
				Fdialog.AddExtension = true;
				Fdialog.FileName = project.title;
				if (Fdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					path = Fdialog.FileName;
				}
			}
			if (path == null)
			{
				return;
			}
			string folder = project.GetPath();
			/*Thread thread = new Thread(() => { CreateSample(path, null, folder); });
			thread.Start();*/
			CreateSample(path, null, folder);
			if(ShowFolder)
				Process.Start(new ProcessStartInfo { FileName = "explorer", Arguments = $"/n,/select,{path}" });
		}

		public static async Task SaveAsZipAsync(Project project, string path = null, bool ShowFolder = true)
		{
			if (path == null)
			{
				return;
			}
			string folder = project.GetPath();
			await Task.Run(() => CreateSample(path, null, folder));
			if (ShowFolder)
				Process.Start(new ProcessStartInfo { FileName = "explorer", Arguments = $"/n,/select,{path}" });
		}

		public static string UploadToFileIO(Project project)
		{
			Task<string> callTask = Task.Run(() => UploadFileIoClient(project));
			callTask.Wait();
			return callTask.Result;
		}

		private static async Task<string> UploadFileIoClient(Project project)
		{
			var title = project.title;
			var savePath = Path.Combine(ProjectManager.temp, title);
			savePath += ".zip";
			SaveAsZip(project, savePath,false);
			using (var httpClient = new HttpClient())
			{
				using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://file.io/?expires=1y"))
				{
					var multipartContent = new MultipartFormDataContent();
					multipartContent.Add(new ByteArrayContent(File.ReadAllBytes(savePath)), "file", Path.GetFileName(savePath));
					request.Content = multipartContent;

					var response = await httpClient.SendAsync(request);
					if (response.IsSuccessStatusCode == true)
					{
						string res = await response.Content.ReadAsStringAsync();
						var content = JsonConvert.DeserializeObject<FileIoResponseModel>(res);
						File.Delete(savePath);
						return content.link;
					}
					return null;
				}
			}
		}

		public static void UnZipProject(string zipFileName = null)
		{
			bool isValidProject = false;
			if (zipFileName == null)
			{
				OpenFileDialog Fdialog = new OpenFileDialog();
				Fdialog.Filter = "Zip (*.zip)|*.zip";
				if (Fdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					zipFileName = Fdialog.FileName;
				}
			}
			if (zipFileName == null)
			{
				return;
			}
			using (ZipFile zf = new ZipFile(zipFileName))
			{
				foreach (ZipEntry ze in zf)
				{
					if (ze.Name.Contains("project.json"))
						isValidProject = true;
				}
			}
			if (!isValidProject)
			{
				return;
			}
			FastZip fastZip = new FastZip();
			string fileFilter = null;
			// Will always overwrite if target filenames already exist
			fastZip.ExtractZip(zipFileName, ProjectManager.saveDir, fileFilter);
		}


		// Compresses the files in the nominated folder, and creates a zip file on disk named as outPathname.
		//
		public static void CreateSample(string outPathname, string password, string folderName)
		{
			if (outPathname == null || outPathname.Count() < 2)
				return;

			if (!outPathname.EndsWith(".zip"))
				outPathname += ".zip";

			FileStream fsOut = File.Create(outPathname);
			ZipOutputStream zipStream = new ZipOutputStream(fsOut);

			zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

			zipStream.Password = password;  // optional. Null is the same as not setting. Required if using AES.

			// This setting will strip the leading part of the folder path in the entries, to
			// make the entries relative to the starting folder.
			// To include the full path for each entry up to the drive root, assign folderOffset = 0.
			int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1) - new DirectoryInfo(folderName).Name.Count() - 1;

			CompressFolder(folderName, zipStream, folderOffset);

			zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
			zipStream.Close();
		}

		// Recurses down the folder structure
		//
		private static void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
		{

			string[] files = Directory.GetFiles(path);

			foreach (string filename in files)
			{

				FileInfo fi = new FileInfo(filename);

				string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
				entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
				ZipEntry newEntry = new ZipEntry(entryName);
				newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

				// Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
				// A password on the ZipOutputStream is required if using AES.
				//   newEntry.AESKeySize = 256;

				// To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
				// you need to do one of the following: Specify UseZip64.Off, or set the Size.
				// If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
				// but the zip will be in Zip64 format which not all utilities can understand.
				//   zipStream.UseZip64 = UseZip64.Off;
				newEntry.Size = fi.Length;

				zipStream.PutNextEntry(newEntry);

				// Zip the file in buffered chunks
				// the "using" will close the stream even if an exception occurs
				byte[] buffer = new byte[4096];
				using (FileStream streamReader = File.OpenRead(filename))
				{
					StreamUtils.Copy(streamReader, zipStream, buffer);
				}
				zipStream.CloseEntry();
			}
			string[] folders = Directory.GetDirectories(path);
			foreach (string folder in folders)
			{
				CompressFolder(folder, zipStream, folderOffset);
			}

		}
	}
	class FileIoResponseModel
	{
		public bool success;
		public string key;
		public string link;
		public string expiry;
	}
}
