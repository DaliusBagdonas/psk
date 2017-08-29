using FluentNHibernate.Mapping;
using PSK.Model.Abstract;

namespace PSK.Model.Questions
{
	public class FreeTextQuestion : AbstractQuestion
	{
	}

	public class FreeTextQuestionMappings : SubclassMap<FreeTextQuestion>
	{
		public FreeTextQuestionMappings()
		{

		}
	}
}