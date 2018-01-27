using DehaxGL;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DehaxRenderer.Views
{
	public partial class MainWindow : Window
	{
		private DehaxGL.DehaxGL _renderer;

		public MainWindow()
		{
			InitializeComponent();

			_renderer = new DehaxGL.DehaxGL(viewport);
			viewport.Renderer = _renderer;
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Left:
					_renderer.Camera.Rotate(0f, Utils.DegreeToRadian(-1f));
					break;
				case Key.Up:
					_renderer.Camera.Rotate(Utils.DegreeToRadian(-1f), 0f);
					break;
				case Key.Right:
					_renderer.Camera.Rotate(0f, Utils.DegreeToRadian(1f));
					break;
				case Key.Down:
					_renderer.Camera.Rotate(Utils.DegreeToRadian(1f), 0f);
					break;
				default:
					break;
			}

			viewport.InvalidateVisual();
		}
	}
}
