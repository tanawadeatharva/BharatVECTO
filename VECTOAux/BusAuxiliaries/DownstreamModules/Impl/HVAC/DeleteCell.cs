using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace TUGraz.VectoCore.BusAuxiliaries.DownstreamModules.Impl.HVAC {
	public class DeleteCell : DataGridViewButtonCell
	{
		public string ToolTip { get; set; } = "Delete tech benefit line";
		private Image del = null;  //= My.Resources.ResourceManager.GetObject("Delete") as Image;


		protected override void Paint(
			Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState,
			object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle,
			DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
		{
			if (del == null) {
				var assembly = Assembly.GetExecutingAssembly();
				var file = assembly.GetManifestResourceStream("Delete");
				del = Image.FromStream(file);
			}
			advancedBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;

			this.ToolTipText = ToolTip;

			cellStyle.BackColor = Color.White;
			base.Paint(
				graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle,
				advancedBorderStyle, paintParts);
			graphics.DrawImage(del, cellBounds);
		}
	}

	public class DeleteAlternatorCell : DataGridViewButtonCell
	{
		public string ToolTip { get; set; } = "Delete alternator";

		protected Image del = null;
		//private Image del = My.Resources.ResourceManager.GetObject("Delete") as Image;

		protected override void Paint(
			Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState,
			object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle,
			DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
		{
			if (del == null) {
				var assembly = Assembly.GetExecutingAssembly();
				var file = assembly.GetManifestResourceStream("Delete");
				del = Image.FromStream(file); 
			}


			advancedBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;

			this.ToolTipText = ToolTip;

			cellStyle.BackColor = Color.White;
			base.Paint(
				graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle,
				advancedBorderStyle, paintParts);
			graphics.DrawImage(del, cellBounds);
		}
	}
}