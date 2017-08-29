using System.Collections.Generic;
using FluentNHibernate.Mapping;
using PSK.Model.Abstract;
using PSK.Model.Identity;

namespace PSK.Model
{
	public class Quiz /*: EntityId<int>*/
	{
		public Quiz() { }

		public Quiz(string name, ApplicationUser owner)
		{
			Name = name;
			CreatedBy = owner;
		}

		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ApplicationUser CreatedBy { get; set; }
		public virtual IList<AbstractQuestion> Questions { get; set; }
	}

	public class QuizMappings : ClassMap<Quiz>
	{
		public QuizMappings()
		{
			Id(x => x.Id);
			Map(x => x.Name);
			References(x => x.CreatedBy);
			HasMany(x => x.Questions);
		}
	}
}
