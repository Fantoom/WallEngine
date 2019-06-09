using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WPEngine;
using WPEngine.WPEngineClasses;
using System.Drawing;
using System.ComponentModel;
using DialogController;

namespace WallEngine
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IViewer
	{
		public IntPtr hWnd { get; private set; }
		private Viewer viewer = new Viewer();

		private Controller controller;
		private ProjectManager PM = new ProjectManager();
		public List<Project> projects = new List<Project>();
		public Project currProject;
		private Properties.Settings settings = Properties.Settings.Default;
		private NotifyIcon nicon = new NotifyIcon();
		private DialogManager dManager = new DialogManager();
		
		public MainWindow()
		{
			//Kills if already running
			if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
			{
				System.Windows.MessageBox.Show("Already Running", "Already Running", MessageBoxButton.OK);
				System.Diagnostics.Process.GetCurrentProcess().Kill();
			}
			
			InitializeComponent();
			InitNotifyIcon();
			if (App.arguments.ContainsKey("minimized") || settings.startInTry)
			{
				this.Hide();
			}
			hWnd = viewer.Handle;
			controller = new Controller(this, hWnd);
			projects = PM.LoadProjects();
			MainWindowLoader();
			if (!String.IsNullOrEmpty(settings.lastProject))
			{
				var loadedProject = Project.ProjectFromJson(settings.lastProject);

				if (File.Exists(Path.Combine(loadedProject.GetPath(),"project.json"))) {
					currProject = loadedProject;
					Play(currProject);
				}

			}

			controller.SetVolume(settings.Volume);
			VolumeSlider.Value = settings.Volume;
		}

	

		public void OpenViewer()
		{
			viewer.Show();
		}

		public void UpdateView()
		{
			MainWindowLoader();
		}
		private void MainWindowLoader()
		{
			ThumbPanel.Children.Clear();
			projects = PM.LoadProjects();
			foreach (var item in projects)
			{
				ProjectThumbViewModel viewModel = new ProjectThumbViewModel
				{
					Image = item.GetPreviewPath(),
					project = item,
					Title = item.title
				};
				ProjectThumb thumb = new ProjectThumb(viewModel);

				System.Windows.Controls.ContextMenu cm = this.FindResource("ThumbContextMenu") as System.Windows.Controls.ContextMenu;
				foreach (System.Windows.Controls.MenuItem cmItem in cm.Items) {
					
					cmItem.Click += HandleMenuitem;

				}
				thumb.ContextMenu = cm;
				//(thumb.ContextMenu.Items[0] as System.Windows.Controls.MenuItem).Click += HandleMenuitem;
				thumb.MouseDown += ThumbClick;
				ThumbPanel.Children.Add(thumb);
				
			}
			if (!projects.Contains(currProject) && projects.Count() > 0)
			{
				SelectProject(projects.Last());
			}
			if (projects.Count() < 1)
			{
				Reset();
			}
		}

		private void HandleMenuitem(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.MenuItem menuItem = e.Source as System.Windows.Controls.MenuItem;
			System.Windows.Controls.ContextMenu parent;
			if (menuItem.Parent is System.Windows.Controls.ContextMenu)
			{
				parent = menuItem.Parent as System.Windows.Controls.ContextMenu;
			}
			else
			{
				parent = (menuItem.Parent as  System.Windows.Controls.MenuItem).Parent as System.Windows.Controls.ContextMenu;
			}
			ProjectThumb thumb = parent.PlacementTarget as ProjectThumb;
			var project = thumb.project;
			var ItemName = menuItem.Name;
			
			switch (ItemName)
			{
				case "Play":
					Play(project);
					break;
				case "ShareAsZip":
					 ShareZip(project);
					break;
				case "UploadFileIo":
					UploadFileio(project);
					break;
				case "Delete":
					DeleteProject(project);
					break;
			}
			
		}
		private async void ShareZip(Project project)
		{
			/*System.Windows.Forms.SaveFileDialog Fdialog = new System.Windows.Forms.SaveFileDialog();
			Fdialog.Filter = "Zip (*.zip)|*.zip";
			Fdialog.DefaultExt = "zip";
			Fdialog.AddExtension = true;
			Fdialog.FileName = project.title;
			string path = null;
			if (Fdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				path = Fdialog.FileName;
			}
			await Task.Run(() => ShareController.SaveAsZipAsync(project, path));
			dManager.ShowText("Saved");*/
			Controller.SaveAsZip(project);
		}
		private  void UploadFileio(Project project)
		{
			var link = Controller.UploadFileio(project);
			dManager.ShowSelectableMessage("Your link",link);
		}

		private void SaveAsZipBtn_Click(object sender, RoutedEventArgs e)
		{
			Controller.SaveAsZip(currProject);

		}

		private void UploadFileIoBtn_Click(object sender, RoutedEventArgs e)
		{
			var link = Controller.UploadFileio(currProject);
			dManager.ShowSelectableMessage("Your link", link);
		}

		private void OpenCM(object sender, RoutedEventArgs e)
		{
			(sender as System.Windows.Controls.Control).ContextMenu.IsOpen = true;
		}

		private void UnZipProjectBtn_Click(object sender, RoutedEventArgs e)
		{
			Controller.UnzipProject();
		}

		


		private void SelectProject(Project project)
		{
			if (projects.Contains(project) && currProject != project)
			{
				currProject = project;
				Play(currProject);
			}
		}

		private void EditorButton_Click(object sender, RoutedEventArgs e)
		{
			EditorWindow editor = new EditorWindow();
			editor.Show();
		}

		private void SettingsButton_Click(object sender, RoutedEventArgs e)
		{
		    Settings settingsView = new Settings();
			settingsView.Show();
		}

		private void ThumbClick(object sender, RoutedEventArgs e)
		{
			var thumb = sender as ProjectThumb;
			SelectProject(thumb.project);
		}

		private void Play(Project project)
		{
			controller.Play(project);
			Project_Preview.Source = Controller.CreateBitmapFromImage(Path.Combine(project.GetPath(), project.preview));
			Project_Title.Content = project.title;
		}
		private void Reset()
		{
			controller.ResetPlayer();
			ResetPreview();
		}
		private void ResetPreview()
		{
			Project_Preview.Source = Controller.CreateBitmapFromImage("Assets/preview_local.jpg");
		}
		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			DeleteProject(currProject);
		}
		private void DeleteProject(Project project)
		{
			controller.Delete(project);
		}
		public void SaveLastProject()
		{
			settings.lastProject = currProject.ToJson();
			settings.Save();
		}

		private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var slider = sender as Slider;
			controller.SetVolume((int)slider.Value);
		}

		private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var slider = sender as Slider;
			if (controller != null)
			controller.SetSpeed((int)slider.Value);
		}
	
		#region TryIcon
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			e.Cancel = true;
			ToTray();

			//Do whatever you want here..
			//Application.Current.Shutdown();
		}

		private void OK_Click(object sender, RoutedEventArgs e)
		{
			ToTray();
		}

		private void ToTray()
		{
			this.Hide();
		}

		private void InitNotifyIcon()
		{
			/*var components = new System.ComponentModel.Container();
			var contextMenu1 = new System.Windows.Forms.ContextMenu();*/
			var exit = new System.Windows.Forms.MenuItem();
			//exit.Index = 0;
			exit.Text = "Exit";
			exit.Click += Exit_Click;

			var isPaused = new System.Windows.Forms.MenuItem();
			//isPaused.Index = 1;
			isPaused.Text = "Pause";
			isPaused.Click += IsPaused_Click;
			
			nicon.DoubleClick += notifyIcon_DoubleClick;

			nicon.ContextMenu = new System.Windows.Forms.ContextMenu();
			nicon.ContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
			 {
				isPaused,
				exit
			 }
			 );
			nicon.Icon = new Icon("Assets/icon.ico");
			nicon.Visible = true;
		}

		private void IsPaused_Click(object sender, EventArgs e)
		{
			var Sender = sender as System.Windows.Forms.MenuItem;
			//Sender.Checked = !Sender.Checked;

			if (controller.isPlaying)
			{
				controller.Pause();
				Sender.Text = "Resume";
			}
			else
			{
				controller.Resume();
				Sender.Text = "Pause";

			}
		}

		private void Exit_Click(object sender, EventArgs e)
		{
			System.Windows.Application.Current.Shutdown();
		}

		private void notifyIcon_DoubleClick(object sender, EventArgs e)
		{
			this.Show();
			this.WindowState = WindowState.Normal;
			this.ShowInTaskbar = true;
		}


		#endregion

		
	}
}
