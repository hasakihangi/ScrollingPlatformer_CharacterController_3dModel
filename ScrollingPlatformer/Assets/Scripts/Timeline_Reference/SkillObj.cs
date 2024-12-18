namespace TimelineReference
{
	public class SkillObj
	{
		public SkillModel model;
	}

	public struct SkillModel
	{
		public string id;
		public ChaResource condition;
		public ChaResource cost;
		public TimelineModel effect;

		public AddBuffInfo[] buff;
		
		
	}
}