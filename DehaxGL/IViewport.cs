namespace DehaxGL
{
	public interface IViewport
	{
		void Begin();
		void SetPixel(int x, int y, uint color);
		void End();
		void SetSize(int width, int height);
		void Clear();

		int Width { get; }
		int Height { get; }
	}
}
