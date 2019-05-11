using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WallEngine
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static Dictionary<string, string> arguments = new Dictionary<string, string>();

		private void Init(object sender, StartupEventArgs e)
		{
			string[] args = Environment.GetCommandLineArgs();

			for (int index = 1; index < args.Length; index += 2)
			{
				string arg = args[index].Replace("--", "").Replace("-","");
				arguments.Add(arg, args[index + 1]);
				Console.WriteLine(arg, args[index + 1]);
			}
		}
		
	}
}
