using System.Windows.Forms;

namespace TUGraz.VectoCore.BusAuxiliaries.DownstreamModules.Impl.HVAC {
	public class DeleteColumn : DataGridViewButtonColumn
	{
		public DeleteColumn() : base()
		{
			this.CellTemplate = new DeleteCell();
		}
	}

	public class DeleteAlternatorColumn : DataGridViewButtonColumn
	{
		public DeleteAlternatorColumn() : base()
		{
			this.CellTemplate = new DeleteAlternatorCell();
		}
	}
}