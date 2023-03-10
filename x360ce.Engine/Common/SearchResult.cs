using x360ce.Engine.Data;

namespace x360ce.Engine
{
	public class SearchResult
	{
		public UserSetting[] Settings { get; set; }
		public Summary[] Summaries { get; set; }
		public PadSetting[] PadSettings { get; set; }
		public Preset[] Presets { get; set; }
	}
}
