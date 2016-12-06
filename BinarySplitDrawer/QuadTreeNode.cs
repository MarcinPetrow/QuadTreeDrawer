using System.Collections.Generic;
using System.Drawing;

namespace QuadTreeDrawer
{
	public class QuadTreeNode : IDrawable
	{
		public List<QuadTreeItem> Items { get; set; }
		public List<QuadTreeItem> UnhandledItems { get; set; }

		private const int MaxItemsPerNode = 100;
		public QuadTreeNode[] Parts;
		public Pen BorderPen { get; set; }
		public int Level { get; set; }

		public Rectangle Area { get; set; }

		public QuadTreeNode()
		{
			Items = new List<QuadTreeItem>();
			UnhandledItems = new List<QuadTreeItem>();
			BorderPen = Pens.OrangeRed;
		}

		public bool IsSplitted()
		{
			return Parts != null;
		}

		public void Split()
		{
			if (IsSplitted())
			{
				return;
			}

			Parts = new QuadTreeNode[4];

			var splitPoint = new Point(Area.X + Area.Width / 2, Area.Y + Area.Height / 2);
			var splitSize = new Size(Area.Width / 2 + 1, Area.Height / 2 + 1);
			CreateEmptyParts(splitSize, splitPoint);

			foreach (var item in Items)
			{
				MoveItemToSplits(item);
			}
			Items.Clear();
		}

		private void CreateEmptyParts(Size splitSize, Point splitPoint)
		{
			Parts[0] = CreatePart(new Rectangle(Area.Location, splitSize), Pens.ForestGreen);
			Parts[1] = CreatePart(new Rectangle(new Point(splitPoint.X, Area.Y), splitSize), Pens.RoyalBlue);
			Parts[2] = CreatePart(new Rectangle(splitPoint, splitSize), Pens.RosyBrown);
			Parts[3] = CreatePart(new Rectangle(new Point(Area.X, splitPoint.Y), splitSize), Pens.BlueViolet);
		}

		private QuadTreeNode CreatePart(Rectangle newArea, Pen partColor)
		{
			return new QuadTreeNode
			{
				Area = newArea,
				BorderPen = partColor,
				Level = Level + 1
			};
		}

		public void AddNode(QuadTreeItem node)
		{
			if (!Area.IntersectsWith(node.Area))
			{
				UnhandledItems.Add(node);
				return;
			}

			if (IsSplitted())
			{
				MoveItemToSplits(node);
				return;
			}

			Items.Add(node);
			if (Items.Count > MaxItemsPerNode)
			{
				Split();
			}
		}

		private void MoveItemToSplits(QuadTreeItem item)
		{
			for (int partId = 0; partId < 4; partId++)
			{
				if (Parts[partId].Area.IntersectsWith(item.Area))
				{
					Parts[partId].AddNode(item);
					return;
				}
			}
			UnhandledItems.Add(item);
		}

		public void Paint(Graphics g, Rectangle area)
		{
			if (Area.IntersectsWith(area))
			{
				g.DrawRectangle(BorderPen, Area);
			}

			if (Parts == null)
			{
				return;
			}
			for (int partId = 0; partId < 4; partId++)
			{
				Parts[partId].Paint(g, area);
			}
		}

		public List<QuadTreeItem> GetItems(Rectangle mouseArea)
		{
			if (!IsSplitted())
			{
				return Items;
			}
			var resultItems = new List<QuadTreeItem>();
			foreach (var part in Parts)
			{
				if (part.Area.IntersectsWith(mouseArea))
				{
					resultItems.AddRange(part.GetItems(mouseArea));

				}
			}

			return resultItems;
		}

		public List<QuadTreeItem> GetAllItems()
		{
			List<QuadTreeItem> resultItems = new List<QuadTreeItem>();
			resultItems.AddRange(UnhandledItems);
			if (!IsSplitted())
			{
				resultItems.AddRange(Items);
				return resultItems;
			}

			foreach (var part in Parts)
			{
				{
					resultItems.AddRange(part.GetAllItems());
				}
			}
			return resultItems;
		}

		public void Cleanup()
		{
			if (IsSplitted())
			{
				foreach (var part in Parts)
				{
					part.Cleanup();
				}
			}
			Items.Clear();
			UnhandledItems.Clear();
			Parts = null;
		}

		public int GetUnhandledCount()
		{
			int result = 0;
			result += UnhandledItems.Count;
			if (!IsSplitted())
			{
				return result;
			}
			foreach (var part in Parts)
			{
				result += part.GetUnhandledCount();
			}
			return result;
		}
	}
}