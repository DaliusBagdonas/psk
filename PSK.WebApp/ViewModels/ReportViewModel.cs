using PSK.Model.Abstract;
using PSK.Model.Answers;
using PSK.Model.Questions;
using System.Collections.Generic;

namespace PSK.WebApp.ViewModels
{
	public class ReportViewModel
	{
        public ReportViewModel(List<FreeText> freeText, List<Integer> integer, List<SingleChoice> singleChoice, List<MultiChoice> multiChoice)
        {
            FreeText = freeText;
            Integer = integer;
            SingleChoice = singleChoice;
            MultiChoice = multiChoice;
        }
        
        public List<FreeText> FreeText { get; set; }
        public List<Integer> Integer { get; set; }
        public List<SingleChoice> SingleChoice { get; set; }
        public List<MultiChoice> MultiChoice { get; set; }
    }

    public class FreeText
    {
        public AbstractQuestion Question { get; set; }
        public List<FreeTextAnswer> FreeTextAnswers { get; set; }
    }

    public class Integer
    {
        public AbstractQuestion Question { get; set; }
        public List<IntegerAnswer> IntegertAnswers { get; set; }
        public double AverageScore { get; set; }
        public int LowestScore { get; set; }
        public int HighestScore { get; set; }
    }

    public class SingleChoice
    {
        public AbstractQuestion Question { get; set; }
        public List<SingleChoiceAnswer> SingleChoiceAnswers { get; set; }
        public List<string> MostCommonAnswers { get; set; }
    }

    public class MultiChoice
    {
        public AbstractQuestion Question { get; set; }
        public List<MultipleChoiceAnswer> MultiChoiceAnswers { get; set; }
        public List<string> MostCommonAnswers { get; set; }
    }
}