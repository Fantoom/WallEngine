using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using WPEngine;
using WPEngine.WPEngineClasses;

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

		public Settings()
		{
			InitializeComponent();
			AutoStartChkbox.IsChecked  = settings.autoStart;
			StartInTryChkbox.IsChecked = settings.startInTry;
			Console.WriteLine("startInTry - " + settings.startInTry.ToString() + "  autoStart - " + settings.autoStart.ToString());
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
	}
}
