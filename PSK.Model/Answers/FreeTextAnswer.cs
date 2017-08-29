using FluentNHibernate.Mapping;
using PSK.Model.Abstract;

namespace PSK.Model.Answers
{
	public class FreeTextAnswer : AbstractAnswer
	{
		public virtual string Answer { get; set; }
	}

	public class FreeTextAnswerMappings : SubclassMap<FreeTextAnswer>
	{
		public FreeTextAnswerMappings()
		{
			Map(x => x.Answer);
		}
	}
}