using System.Drawing;

namespace QuadTreeDrawer
{
	public class QuadTreeItem : IDrawable
	{
		public Rectangle Area;

		public void Paint(Graphics g)
		{
			g.DrawRectangle(Pens.Black, Area.X, Area.Y, Area.Width, Area.Height);
		}

		public void Paint(Graphics g, Rectangle area)
		{
			if (Area.IntersectsWith(area))
			{
				g.DrawRectangle(Pens.Black, Area.X, Area.Y, Area.Width, Area.Height);
			}
		}
	}
}