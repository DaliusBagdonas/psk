using System;
using FluentNHibernate.Mapping;
using NHibernate.Mapping.ByCode.Conformist;

namespace PSK.Model.Abstract
{
	public abstract class AbstractAnswer /*: EntityId<int>*/
	{
		public virtual int Id { get; set; }
		public virtual AbstractQuestion AnswerTo { get; set; }
		public virtual DateTime Date { get; set; }
		public virtual string CorrelationId { get; set; }
	}

	public class AbstractAnswerMappings : ClassMap<AbstractAnswer>
	{
		public AbstractAnswerMappings()
		{
			UseUnionSubclassForInheritanceMapping();
			Id(x => x.Id).GeneratedBy.HiLo("1000");
			References(x => x.AnswerTo);
			Map(x => x.CorrelationId);
			Map(x => x.Date);
		}
	}
}