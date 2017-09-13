namespace HashingTool.ViewModel
{
	public class ComponentEntry
	{
		public string Component { get; set; }
		public string DigestValueRead { get; set; }
		public string DigestValueComputed { get; set; }
		public string[] CanonicalizationMethod { get; set; }
		public bool Valid { get; set; }
	}
}
