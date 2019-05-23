using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wallpainter;
using Mpv.NET.Player;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using WallEngine;
namespace WPEngine.WPEngineClasses
{
	class Controller
	{
		private RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

		private WallEngine.Properties.Settings settings = WallEngine.Properties.Settings.Default;

		private WallpaperManager Wmanager = new WallpaperManager();
		public  int screenW, screenH;
		private int x, y, w, h;
		private IViewer view;
		private IntPtr Handle;
		private int volume;
		public  int Volume { get => volume; set { volume = value; player.Volume = value; } }
		private double speed;
		private bool stopifMiximized;
		public bool StopIfMaximized { get => settings.stopIfMaximized; set { settings.stopIfMaximized = value; stopifMiximized = value; } }

		public double Speed
		{
			get => speed;

			set
			{
				if (value > 0 && value <= 1)
				{
					speed = value; player.Speed = value;
				}
				else if(value > 1)
				{
					speed = value / 100; player.Speed = value / 100;
				}
			}
		}

		private ProjectManager PM = new ProjectManager();
		private MpvPlayer player;
		public static Controller instance;
		public bool isPlaying;

		public Controller(IViewer view, IntPtr handle = default(IntPtr), int volume = 0 , int screenWidth = 0, int screenHeight = 0)
		{
			this.view = view;
			Handle = handle == default(IntPtr) ? handle : view.hWnd;

			player = new MpvPlayer(Handle)
			{
				Loop = true,
				Volume = 0,
			};
			player.EnableYouTubeDl();
			isPlaying = player.IsPlaying;

			player.Volume = volume;
			screenW = screenWidth  == 0 ? Screen.PrimaryScreen.Bounds.Width  : screenWidth;
			screenH = screenHeight == 0 ? Screen.PrimaryScreen.Bounds.Height : screenHeight;
			x = 0;
			y = 0;
			w = screenW;
			h = screenH;
			
			if (Handle == IntPtr.Zero)
			{
				MessageBox.Show("No window found!", "Failed to attach", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			
			if (Wmanager.SetWallpaper(Handle, x, y, w, h))
			{
				view.OpenViewer();
			}
			else
			{
				MessageBox.Show("FILED", "FILED", MessageBoxButtons.OK, MessageBoxIcon.Error);

			}
			instance = this;
			Console.WriteLine(player.Speed);
			
		}

		private void LoadSettings()
		{
			stopifMiximized = settings.stopIfMaximized;
		}

		public void CreateProject(string filePath, string previewPath, string title , string uri = "")
		{
			if (File.Exists(filePath) || Controller.CheckURL(uri))
			{
				PM.CreateProject(filePath, previewPath, title,uri);
			}
		}

		public void SetAutoStart(bool state)
		{
			settings.autoStart = state;
			settings.Save();
			ManageAutostart();
		}
		public void SetStartInTry(bool state)
		{
			settings.startInTry = state;
			settings.Save();
		
		}

		private void ManageAutostart()
		{
			if(settings.autoStart)
				rkApp.SetValue("WallEngine", Application.ExecutablePath + " -minimized");
			else
				rkApp.DeleteValue("WallEngine", false);
		}

		public void Delete(Project project)
		{
			PM.DeleteProject(project);
			view.SaveLastProject();
		}

		public void UpdateView()
		{
			view.UpdateView();
		}

		public void Play(string Path)
		{
			player.Load(Path);
			player.Resume();
			isPlaying = true;
		}
		public void Play(Project project)
		{
				
			var file = Path.Combine(project.GetPath(), project.file);
			var uri = project.URI;
			if (!CheckURL(uri)) { 
			if (!File.Exists(file))
			{
				return;
			}
			}
			if (CheckURL(uri))
			{
				file = uri;
			}
			player.Load(file,true);
			player.Resume();
			view.SaveLastProject();
			isPlaying = true;

		}
		
		public void Resume()
		{
			player.Resume();
			isPlaying = true;
		}
		public void Pause()
		{
			player.Pause();
			isPlaying = false;
		}
		public void SetVolume(int value)
		{
			player.Volume = value;
		}
		public void SetSpeed(double value)
		{
			Speed = value;
		}
		public static Controller getInstance()
		{
			return instance;
		}
		public void ResetPlayer()
		{
		   player.Stop();
		   player.Load("Assets/black-square.jpg");
		}

		public static bool CheckURL(string URL = "")
		{
			if (URL.Count() > 0 && Uri.IsWellFormedUriString(URL, UriKind.Absolute) && URL.Contains("http"))
			{
				return true;
			}
			return false;
		}

		public static void SaveAsZip(Project project)
		{
			ShareController.SaveAsZip(project);
		}
		public static string UploadFileio(Project project)
		{
			return ShareController.UploadToFileIO(project);
		}
		public static void UnzipProject()
		{
			ShareController.UnZipProject();
			Controller.getInstance().UpdateView();
		}
		public static BitmapImage CreateBitmapFromImage(string value, bool isBaseDir = true)
		{
			var dir = ProjectManager.baseDir;
			if (!isBaseDir)
				 dir = "";

			var Bitmap = new BitmapImage();
			Bitmap.BeginInit();
			Bitmap.UriSource = new Uri(System.IO.Path.Combine(dir, value));
			Bitmap.DecodePixelWidth = 256;
			Bitmap.CacheOption = BitmapCacheOption.OnLoad;
			Bitmap.EndInit();
			return Bitmap;
		}


	}
}
