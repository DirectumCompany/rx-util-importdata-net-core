#pragma warning disable CS1591

namespace ClientApp.Models
{
	public class ControlState
	{
		public bool IsVisible { get; set; } = true;
		public bool IsRequired { get; set; } = true;
		public string RequiredError { get; set; }
	}
}
