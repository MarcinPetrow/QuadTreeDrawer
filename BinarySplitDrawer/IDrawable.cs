using System.Drawing;

namespace QuadTreeDrawer
{
	public interface IDrawable
	{
		void Paint(Graphics g, Rectangle area);
	}
}