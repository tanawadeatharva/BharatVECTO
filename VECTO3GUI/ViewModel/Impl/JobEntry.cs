namespace VECTO3.ViewModel.Impl {
	public class JobEntry : ObservableObject
	{
		private bool _selected;
		private string _filename;

		public int Sorting;
		public bool Selected { get { return _selected; } set { SetProperty(ref _selected, value); } }
		public string Filename { get { return _filename; } set { SetProperty(ref _filename, value); } }
	}
}