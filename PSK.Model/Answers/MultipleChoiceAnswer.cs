using System.Collections.Generic;
using FluentNHibernate.Mapping;
using PSK.Model.Abstract;
using PSK.Model.Questions;

namespace PSK.Model.Answers
{
	public class MultipleChoiceAnswer : AbstractAnswer
	{
		public virtual IList<ChoiceOption> Answer { get; set; }
	}

	public class MultipleChoiceAnswerMappings : SubclassMap<MultipleChoiceAnswer>
	{
		public MultipleChoiceAnswerMappings()
		{

			HasMany(x => x.Answer).Cascade.All();
		}
	}
}