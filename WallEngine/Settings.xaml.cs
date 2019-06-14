using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using WPEngine.WPEngineClasses;
using System.Diagnostics;
using System.Windows.Navigation;

namespace WallEngine
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>

	public partial class Settings : Window
	{
		private Properties.Settings settings = Properties.Settings.Default;

		private bool startInTry { get { return settings.startInTry; }  set { Controller.instance.SetStartInTry(value); } }
		private bool autoStart  { get { return settings.autoStart;  }  set { Controller.instance.SetAutoStart(value); } }
		private bool stopIfMaximized { get { return settings.stopIfMaximized; } set { Controller.instance.SetAutoStart(value); } }
		private SwatchesProvider swatchesProvider = new SwatchesProvider();
		private PaletteHelper paletteHelper = new PaletteHelper();
		private static string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		public static string Version { get => version; }
		public Settings()
		{
			InitializeComponent();
			AutoStartChkbox.IsChecked  = settings.autoStart;
			StartInTryChkbox.IsChecked = settings.startInTry;
			Console.WriteLine("startInTry - " + settings.startInTry.ToString() + "  autoStart - " + settings.autoStart.ToString());
			DataContext = this;
			List<string> PrimaryColorsList = swatchesProvider.Swatches.Select(a => a.Name).ToList();
			
			foreach (var color in PrimaryColorsList)
			{
			primaryPaletteComboBox.Items.Add(color);
			}
			
		}
		

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			settings.Save();

		}

		private void StartInTryChkbox_Click(object sender, RoutedEventArgs e)
		{
			startInTry = (bool)((sender as CheckBox).IsChecked);
			Console.WriteLine("startInTry - " + settings.startInTry.ToString() + "  autoStart - " + settings.autoStart.ToString());

		}


		private void AutoStartChkbox_Click(object sender, RoutedEventArgs e)
		{
			autoStart = (bool)((sender as CheckBox).IsChecked);
			Console.WriteLine("startInTry - " + settings.startInTry.ToString() + "  autoStart - " + settings.autoStart.ToString());


		}
		

		private void primaryPaletteComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			SwatchesProvider swatchesProvider = new SwatchesProvider();
			Swatch color = swatchesProvider.Swatches.Where(a => a.Name == primaryPaletteComboBox.SelectedItem.ToString()).FirstOrDefault();
			/*foreach (var item in swatchesProvider.Swatches)
			{
				if (item.Name == primaryPaletteComboBox.SelectedItem.ToString())
					color = item; 
				else
					color = swatchesProvider.Swatches.Select()
			} */
			paletteHelper.ReplacePrimaryColor(color);
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}
	}
}
