using FluentNHibernate.Mapping;
using PSK.Model.Abstract;
using PSK.Model.Questions;

namespace PSK.Model.Answers
{
	public class SingleChoiceAnswer : AbstractAnswer
	{
		public virtual ChoiceOption Answer { get; set; }
	}

	public class SingleChoiceAnswerMappings : SubclassMap<SingleChoiceAnswer>
	{
		public SingleChoiceAnswerMappings()
		{


			References(x => x.Answer);
		}
	}
}