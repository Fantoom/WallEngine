﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace WPEngine.WPEngineClasses
{
	public class Project
	{
		public string file;
		public string preview;
		public string title;
		public string audio;
		public string URI;

		public Project()
		{

		}
		public Project(string file, string preview,string title, string uri = "", string audio = "")
		{
			this.file = file;
			this.preview = preview;
			this.title = title;
			this.URI = uri;
			this.audio = audio;
		}

		public string GetPath()
		{
			return Path.Combine(ProjectManager.saveDir, title);
		}
		public string GetFilePath()
		{
			return Path.Combine(ProjectManager.saveDir, title, file);
		}
		public string GetPreviewPath()
		{
			return Path.Combine(ProjectManager.saveDir, title, preview);
		}
		public string GetAudioPath()
		{
			return Path.Combine(ProjectManager.saveDir, title, audio);
		}

		public static string ToJson(Project project)
		{
			return JsonConvert.SerializeObject(project, Formatting.Indented);
		}
		public static Project ProjectFromJson(string json)
		{
			return JsonConvert.DeserializeObject<Project>(json);
		}

		public string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}



	}
}
