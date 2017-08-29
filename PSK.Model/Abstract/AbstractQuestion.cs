using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace PSK.Model.Abstract
{
	public abstract class AbstractQuestion /*: EntityId<int>*/
	{
		public virtual int Id { get; set; }
		public virtual bool Mandatory { get; set; }
		public virtual string QuestionText { get; set; }
		public virtual Quiz Quiz { get; set; }
	}

	public class AbstractQuestionMappings : ClassMap<AbstractQuestion>
	{
		public AbstractQuestionMappings()
		{
			UseUnionSubclassForInheritanceMapping();
			Id(x => x.Id).GeneratedBy.HiLo("1000");
			Map(x => x.Mandatory);
			Map(x => x.QuestionText);
			References(x => x.Quiz).Cascade.All();
		}
	}
}