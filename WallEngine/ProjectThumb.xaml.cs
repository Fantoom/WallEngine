﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPEngine.WPEngineClasses;
using System.IO;
using System.Windows.Media.Animation;

namespace WallEngine
{
	/// <summary>
	/// Interaction logic for ProjectThumb.xaml
	/// </summary>
	public partial class ProjectThumb : UserControl
	{
		public Project project;
		private string imagePath;
		private string title;
		public string Image { get { return imagePath; } set { imagePath = value; PreviewImage.Source = CreateBitmap(value); } }
		public string Title { get { return title; } set { title = value;  } }
		private double normalOpacity = 0.4f;
		private double hoverdOpacity = 0.7f;
		private BitmapImage Bmap = new BitmapImage();
		public ProjectThumb()
		{
			InitializeComponent();
			DataContext = this;
		}
		public ProjectThumb(ProjectThumbViewModel viewModel)
		{
			InitializeComponent();
			project = viewModel.project;
			Image = viewModel.Image;
			Title = viewModel.Title;
			DataContext = viewModel;
		}

		#region Other
		private void Grid_MouseEnter(object sender, MouseEventArgs e)
		{
			//rect.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B2000000"));
			rect.Opacity = hoverdOpacity;
		}

		private void Grid_MouseLeave(object sender, MouseEventArgs e)
		{
			//rect.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#66000000"));
			rect.Opacity = normalOpacity;
		}
		private BitmapImage CreateBitmap(string value)
		{
			Bmap = new BitmapImage(new Uri(System.IO.Path.Combine(ProjectManager.saveDir, value)));
			Bmap.DecodePixelWidth = 256;
			return Bmap;
		}
		public static BitmapImage CreateBitmapFromImage(string value)
		{
			var Bitmap = new BitmapImage(new Uri(System.IO.Path.Combine(ProjectManager.saveDir, value)));
			Bitmap.DecodePixelWidth = 256;
			return Bitmap;
		}
		#endregion

	}
	public class ProjectThumbViewModel
	{
		public string Image { get; set; } = "Images/preview_local.jpg";
		public string Title { get; set; } = "Project Title";
		public Project project { get; set; }
	}
}