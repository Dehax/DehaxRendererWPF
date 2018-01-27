using DehaxGL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Diagnostics;

namespace DehaxRenderer.Views.Controls
{
	public class Viewport : FrameworkElement, IViewport
	{
		private WriteableBitmap _image;
		//private byte[] _rawImage;
		private int _rawStride;
		private IntPtr _backBufferPtr;

		public Viewport()
		{
		}

		int IViewport.Width => (int)ActualWidth;
		int IViewport.Height => (int)ActualHeight;

		private int _width;
		private int _height;

		public DehaxGL.DehaxGL Renderer;

		public unsafe void Clear()
		{
			_image.Lock();

			uint color = 0;

			for (int i = 0; i < _height; i++)
			{
				int pBackBuffer = (int)_backBufferPtr + i * _rawStride;

				for (int j = 0; j < _width; j++)
				{
					color |= 0xffu << 24;
					color |= 0x80u << 16;
					color |= 0x80u << 8;
					color |= 0x80u << 0;

					*((uint*)pBackBuffer) = color;

					pBackBuffer += 4;
				}
			}

			_image.AddDirtyRect(new Int32Rect(0, 0, _width, _height));
			
			_image.Unlock();
		}

		public void Begin()
		{
			_image.Lock();
		}

		public unsafe void SetPixel(int x, int y, uint color)
		{
			int h = _height - y;

			if (x >= 0 && x < _width && h >= 0 && h < _height)
			{
				*((uint*)(_backBufferPtr + h * _rawStride + x * 4)) = color;
			}
		}

		public void End()
		{
			_image.AddDirtyRect(new Int32Rect(0, 0, _width, _height));

			_image.Unlock();
		}

		public void SetSize(int width, int height)
		{
			_width = width;
			_height = height;

			PixelFormat pf = PixelFormats.Bgra32;
			_rawStride = (width * pf.BitsPerPixel + 7) / 8;
			//_rawImage = new byte[_rawStride * height];
			
			_image = new WriteableBitmap(width, height, 96, 96, pf, null);
			_backBufferPtr = _image.BackBuffer;
			Clear();
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			if (_image == null)
				return;

			//var watch = System.Diagnostics.Stopwatch.StartNew();

			long StartingTime = Stopwatch.GetTimestamp();

			Renderer.Render(DehaxGL.DehaxGL.RenderMode.Solid);

			long EndingTime = Stopwatch.GetTimestamp();
			long ElapsedTime = EndingTime - StartingTime;

			double ElapsedSeconds = (ElapsedTime * 1000000) / Stopwatch.Frequency;

			//watch.Stop();
			//long elapsedMs = watch.ElapsedMilliseconds;

			drawingContext.DrawImage(_image, new Rect(RenderSize));
			drawingContext.DrawText(new FormattedText((ElapsedSeconds).ToString("F0"), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Consolas"), 12, Brushes.Black, 1.0), new Point(0.0, 0.0));
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			Size result = base.ArrangeOverride(finalSize);

			Renderer?.SetViewportSize((int)result.Width, (int)result.Height);

			return result;
		}
	}
}
