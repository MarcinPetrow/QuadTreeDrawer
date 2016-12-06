using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuadTreeDrawer
{
	public partial class MainForm : Form
	{
		private readonly QuadTreeTree treeTree;

		public Rectangle mouseArea;

		private Font defaultFont = new Font("Verdana", 10);

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000;
				return cp;
			}
		}

		public MainForm()
		{
			SetStyle(
			ControlStyles.UserPaint |
			ControlStyles.AllPaintingInWmPaint |
			ControlStyles.OptimizedDoubleBuffer, true);

			InitializeComponent();

			treeTree = new QuadTreeTree
			{
				Area = GetWorkingArea()
			};

			Random random = new Random(1);
			for (int i = 0; i < 1000000; i++)
			{
				var node = new QuadTreeItem
				{
					Area = new Rectangle(random.Next(1600), random.Next(800), 20, 20)

				};
				treeTree.AddNode(node);
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			mouseArea = new Rectangle(e.Location, new Size(50, 50));
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			var graphics = e.Graphics;

			if (treeTree != null)
			{
				treeTree.Paint(graphics, mouseArea);

				var items = treeTree.GetItems(mouseArea);
				foreach (var item in items)
				{
					item.Paint(graphics);
				}

				graphics.DrawRectangle(Pens.Orange, mouseArea);

				graphics.DrawString($"Rendered elements: {items.Count} from 1000000 total", defaultFont, Brushes.Black, 10, 10);
			}
			graphics.DrawString($"Active area size: {ClientRectangle}", defaultFont, Brushes.Black, 10, 22);
			graphics.DrawString($"Visible area: {mouseArea}", defaultFont, Brushes.Black, 10, 34);
			if (treeTree != null)
			{
				graphics.DrawString($"Unhandled items count: {treeTree.GetUnhandledCount()}", defaultFont, Brushes.Black, 10, 46);
			}

		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			treeTree?.Resize(GetWorkingArea());
		}

		private Rectangle GetWorkingArea()
		{
			return new Rectangle(ClientRectangle.X + 5, ClientRectangle.Y + 5, ClientRectangle.Width - 10, ClientRectangle.Height - 10);
		}
	}
}
