using System.Collections.Generic;
using FluentNHibernate.Mapping;
using PSK.Model.Abstract;

namespace PSK.Model.Questions
{
	public class SingleChoiceQuestion : AbstractQuestion
	{
		public virtual IList<ChoiceOption> Options { get; set; }
	}

	public class SingleChoiceQuestionMappings : SubclassMap<SingleChoiceQuestion>
	{
		public SingleChoiceQuestionMappings()
		{

			HasMany(x => x.Options).Cascade.All();
		}
	}
}