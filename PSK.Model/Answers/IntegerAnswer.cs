using FluentNHibernate.Mapping;
using PSK.Model.Abstract;

namespace PSK.Model.Answers
{
	public class IntegerAnswer : AbstractAnswer
	{
		public virtual int Answer { get; set; }
	}

	public class IntegerAnswerMappings : SubclassMap<IntegerAnswer>
	{
		public IntegerAnswerMappings()
		{

			Map(x => x.Answer);
		}
	}
}