using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//using System.Windows.Shapes;
using Mpv.NET.Player;
using WPEngine.WPEngineClasses;
using System.IO;
using System.Windows.Forms;
using DialogController;
using System.Globalization;

namespace WallEngine
{
	/// <summary>
	/// Interaction logic for EditorWindow.xaml
	/// </summary>
	public partial class EditorWindow : Window
	{
		private MpvPlayer player;
		private string fileName;
		private string filePath;
		private string previewName;
		private string previewPath = "Assets/preview_local.jpg";
		private string URI = "";
		private DialogManager dManager = new DialogManager();
		private ProjectManager PM = new ProjectManager();
		private Controller controller;
		private int _noOfErrorsOnScreen = 0;
		//private bool ValidateTest { get { return Title.} }
		public EditorWindow()
		{
			InitializeComponent();
			this.DataContext = this;
			player = new MpvPlayer(VideoPlayer.Handle)
			{
				Loop = true,
				Volume = 0,
			};
			controller = Controller.getInstance();
		}

		private void SaveProject()
		{
			controller.CreateProject(filePath, previewPath, Title.Text, URI);
		}

		private void SetFile_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog Fdialog = new OpenFileDialog();
			Fdialog.Filter = "Video Files (*.flv *.gif *.avi *.wmv *.amv *.mp4)|*.flv;*.gif;*.avi;*.wmv;*.amv;*.mp4;" + "|All Files|*.*";
			if (Fdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				SetUri.IsEnabled = false;	
				filePath = Fdialog.FileName;
				fileName = Path.GetFileNameWithoutExtension(Fdialog.FileName);
				Console.WriteLine(filePath);
				Console.WriteLine(fileName);
				PathLabel.Content = filePath;
				Title.Text = fileName;
				Save.IsEnabled = true;
				player.Load(filePath);
				player.Resume();
				
			}
		}

		private void SetPreview_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog Fdialog = new OpenFileDialog();
			Fdialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png;" + "|All Files|*.*";
			if (Fdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				previewPath = Fdialog.FileName;
				previewName = Path.GetFileNameWithoutExtension(Fdialog.FileName);
				//BitmapImage Bmap = new BitmapImage(new Uri(previewPath));
				//Bmap.DecodePixelWidth = 256;
				Preview.Source = Controller.CreateBitmapFromImage(previewPath,false);
			}
		}
		private async void SetUri_Click(object sender, RoutedEventArgs e)
		{
			URI = await dManager.InputDialog("Enter Video's URL adress");
			if (Controller.CheckURL(URI))
			{
			filePath = "";
			PathLabel.Content = URI;
			Title.Text = ProjectManager.GenerateRandomID();
			Save.IsEnabled = true;
			player.Load(URI);
			player.Resume();

			}
		}
		private void Save_Click(object sender, RoutedEventArgs e)
		{
			SaveProject();
			this.Close();
		}
		private void VideoPlayer_Click(object sender, EventArgs e)
		{
			if (player.IsMediaLoaded)
			{
				if (player.IsPlaying)
				{
					player.Pause();
					PlayIcon.Visibility = Visibility.Visible;
				}
				else
				{
					player.Resume();
					PlayIcon.Visibility = Visibility.Hidden;
				}
			}
		}

		private void Title_Error(object sender, ValidationErrorEventArgs e)
		{
			if (e.Action == ValidationErrorEventAction.Added)
				_noOfErrorsOnScreen++;
			else
				_noOfErrorsOnScreen--;

			if (_noOfErrorsOnScreen < 1 && (fileName.Count() > 2 || Controller.CheckURL(URI))) { 
			Save.IsEnabled =  true;
			}
			
		}
	}
	public class TitleValidation : ValidationRule
	{
	
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			var str = value as string;
			if (str.Count() < 1)
			{
				return new ValidationResult(false, "Please enter the title");
			}
			if (str.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1)
			{
				Console.WriteLine("VALIDATION WORKED ********************************");
				return new ValidationResult(false, "The title must not contain \\ / : ? * | \" < >");
			}
			return new ValidationResult(true, null);

		}
	}
	
}
