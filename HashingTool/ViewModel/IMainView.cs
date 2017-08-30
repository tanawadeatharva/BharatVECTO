using System.Windows.Input;

namespace HashingTool.ViewModel
{
	public interface IMainView
	{
		string Name { get; }

		ICommand ShowHomeViewCommand { get; }
	}
}
