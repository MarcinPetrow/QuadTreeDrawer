using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QuadTreeDrawer
{
	public partial class MainForm : Form
	{
		private int mouseAreaSize = 150;

		private readonly Font defaultFont = new Font("Verdana", 10);
		private readonly QuadTreeTree tree;
		private readonly Random random;

		public Rectangle MouseArea;

		public MainForm()
		{
			SetStyle(
				ControlStyles.UserPaint |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.OptimizedDoubleBuffer, true);

			InitializeComponent();

			tree = new QuadTreeTree
			{
				Area = GetWorkingArea()
			};

			random = new Random(1);
			for (var i = 0; i < 10000; i++)
			{
				var node = new QuadTreeItem
				{
					Area = new Rectangle(random.Next(1600), random.Next(800), 20, 20)
				};
				tree.AddNode(node);
			}
		}

		protected override CreateParams CreateParams
		{
			get
			{
				var cp = base.CreateParams;
				cp.ExStyle |= 0x02000000;
				return cp;
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			MouseArea = new Rectangle(e.X - mouseAreaSize / 2, e.Y - mouseAreaSize / 2, mouseAreaSize, mouseAreaSize);
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			var graphics = e.Graphics;
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			if (tree != null)
			{
				tree.Paint(graphics, MouseArea);

				var items = tree.GetItems(MouseArea);
				foreach (var item in items)
				{
					item.Paint(graphics);
				}

				graphics.DrawRectangle(Pens.Orange, MouseArea);
				graphics.DrawRectangle(Pens.Orange,
					new Rectangle(MouseArea.X + 5, MouseArea.Y + 5, MouseArea.Width - 10, MouseArea.Height - 10));

				graphics.DrawString($"Rendered elements: {items.Count} from {tree.GetTotalCount()} total", defaultFont, Brushes.Black, 10, 10);
			}
			graphics.DrawString($"Active area size: {ClientRectangle}", defaultFont, Brushes.Black, 10, 22);
			graphics.DrawString($"Visible area: {MouseArea}", defaultFont, Brushes.Black, 10, 34);
			if (tree != null)
			{
				graphics.DrawString($"Unhandled items count: {tree.GetUnhandledCount()}", defaultFont, Brushes.Black, 10, 46);
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			tree?.Resize(GetWorkingArea());
		}

		private Rectangle GetWorkingArea()
		{
			return new Rectangle(ClientRectangle.X + 5, ClientRectangle.Y + 5, ClientRectangle.Width - 10,
				ClientRectangle.Height - 10);
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);

			if (e.Button == MouseButtons.Left)
			{
				for (var i = 0; i < 10000; i++)
				{
					var node = new QuadTreeItem
					{
						Area = new Rectangle(random.Next(1600), random.Next(800), 20, 20)
					};
					tree.AddNode(node);
				}
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			if (e.Delta != 0)
			{
				mouseAreaSize += e.Delta / 30;

				if (mouseAreaSize < 10)
				{
					mouseAreaSize = 10;
				}
			}
			Invalidate();
		}
	}
}