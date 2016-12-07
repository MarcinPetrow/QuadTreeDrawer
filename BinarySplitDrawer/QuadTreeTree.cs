using System.Drawing;

namespace QuadTreeDrawer
{
	public class QuadTreeTree : QuadTreeNode
	{
		public new void Paint(Graphics g, Rectangle renderRegion)
		{
			base.Paint(g, renderRegion);
			if (Area.IntersectsWith(renderRegion))
			{
				g.DrawRectangle(Pens.Red, Area);
			}
		}

		public void Resize(Rectangle newArea)
		{
			var collection = GetAllItems();
			Cleanup();
			Area = newArea;

			foreach (var item in collection)
			{
				AddNode(item);
			}
		}
	}
}