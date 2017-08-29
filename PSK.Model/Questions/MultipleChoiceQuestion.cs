using System.Collections.Generic;
using FluentNHibernate.Mapping;
using PSK.Model.Abstract;

namespace PSK.Model.Questions
{
	public class MultipleChoiceQuestion : AbstractQuestion
	{
		public virtual IList<ChoiceOption> Options { get; set; }
	}

	public class MultipleChoiceQuestionMappings : SubclassMap<MultipleChoiceQuestion>
	{
		public MultipleChoiceQuestionMappings()
		{


			HasMany(x => x.Options).Cascade.All();
		}
	}
}