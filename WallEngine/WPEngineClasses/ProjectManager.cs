﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WPEngine.WPEngineClasses
{
	class ProjectManager
	{
		public static string folderName = "MyProjects";
		public static string baseDir;
		public static string saveDir;
		public static string temp;

		List<Project> Projects = new List<Project>();

		public ProjectManager()
		{
			baseDir = AppDomain.CurrentDomain.BaseDirectory;
			saveDir = Path.Combine(baseDir, folderName);
			temp = Path.Combine(baseDir, "temp");
			if (!Directory.Exists(saveDir))
			{
				Directory.CreateDirectory(saveDir);
			}
			if (!Directory.Exists(temp))
			{
				Directory.CreateDirectory(temp);
			}
		}

		public CreatReturn CreateProject(string filePath, string previewPath, string title, string uri, string audio = "")
		{
			CreatReturn msg = new CreatReturn { Path = "", Message = "", isSuccess = true };
			if (filePath.Count() < 1 || !Controller.CheckURL(uri))
			{
				if (previewPath.Count() < 1 || title.Count() < 1)
				{
					Console.WriteLine("Invalid data");
					msg.Message = "Invalid data";
					msg.isSuccess = false;
					return msg;
				}
			}

			Project project;

			string projectPath = Path.Combine(saveDir, title);
			string projectJsonPath = Path.Combine(projectPath, "project.json");
			string fileName = Path.GetFileName(filePath);
			string fileDestinationPath = Path.Combine(projectPath, fileName);
			string audioName = Path.GetFileName(audio);
			string audioDestinationPath = Path.Combine(projectPath, audioName);

			string URI = "";
			string previewName = Path.GetFileName(previewPath);
			string previewDestinationPath = Path.Combine(projectPath, "preview.jpg");
			if (uri.Length > 0 && Uri.IsWellFormedUriString(uri, UriKind.Absolute) && uri.Contains("http"))
			{
				URI = uri;
			}
			if (!Directory.Exists(projectPath))
			{

				Directory.CreateDirectory(projectPath);
				if(filePath.Count() > 1) {
				File.Copy(filePath, fileDestinationPath);
				}
				if (audio.Count() > 1)
				{
					File.Copy(audio, audioDestinationPath);
				}
				File.Copy(previewPath, previewDestinationPath);
				project = new Project(fileName, "preview.jpg", title, URI, audioName);
				string json = JsonConvert.SerializeObject(project, Formatting.Indented);

				using (FileStream fs = File.Create(projectJsonPath))
				{
					Byte[] info = new UTF8Encoding(true).GetBytes(json);
					// Add some information to the file.
					fs.Write(info, 0, info.Length);
				}

				msg.Path = fileDestinationPath;
				msg.Message = "OK";
				msg.isSuccess = true;
				Controller.getInstance().UpdateView();
			}
			else
			{
				Console.WriteLine("Already exists");
				msg.Message = "Already exists";
				msg.isSuccess = false;
				return msg;
			}

			return msg;
		}
		public void DeleteProject(Project project)
		{

			Projects.Remove(project);
			if(File.Exists(Path.Combine(project.GetPath(), "project.json")))
			{
			File.Delete(Path.Combine(project.GetPath(), "project.json"));
			}
			Controller.getInstance().UpdateView();
			if (Directory.Exists(project.GetPath()))
			{
				Directory.Delete(project.GetPath(), true);
			}
			
		}

		public List<Project> LoadProjects()
		{
			List<Project> projects = new List<Project>();

			string[] filePaths = Directory.GetFiles(saveDir, "project.json", SearchOption.AllDirectories);
			foreach (var path in filePaths)
			{
				Project project = JsonConvert.DeserializeObject<Project>(File.ReadAllText(path));
				project.directory = Path.GetDirectoryName(path);
				projects.Add(project);
			}
			Projects = projects;
			return projects;
		}

		public struct CreatReturn
		{
			public string Path;
			public string Message;
			public bool isSuccess;
		}
		public static string GenerateRandomID()
		{
			int id;
			Random rnd = new Random();
			id = rnd.Next(100000000, 999999999);
			while (Directory.Exists(Path.Combine(saveDir, id.ToString())))
			{
				id = rnd.Next(100000000, 999999999);
			}
			return id.ToString();
		}
	}
}