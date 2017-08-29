using FluentNHibernate.Mapping;
using PSK.Model.Abstract;

namespace PSK.Model.Questions
{
	public class IntegerQuestion : AbstractQuestion
	{
		public virtual int UpperBound { get; set; }
		public virtual int LowerBound { get; set; }
	}

	public class IntegerQuestionMappings : SubclassMap<IntegerQuestion>
	{
		public IntegerQuestionMappings()
		{


			Map(x => x.UpperBound);
			Map(x => x.LowerBound);
		}
	}
}