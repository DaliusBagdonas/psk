using FluentNHibernate.Mapping;
using PSK.Model.Abstract;

namespace PSK.Model.Questions
{
	public class ChoiceOption /*: EntityId<int>*/
	{
		public virtual int Id { get; set; }
		public virtual string Text { get; set; }
	}

	public class ChoiceOptionMappings : ClassMap<ChoiceOption>
	{
		public ChoiceOptionMappings()
		{
			Id(x => x.Id);
			Map(x => x.Text);
		}
	}
}