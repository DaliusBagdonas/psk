using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using NHibernate.AspNet.Identity;
using NHibernate.Util;
using PSK.Model;
using PSK.Model.Abstract;
using PSK.Model.Identity;
using PSK.Model.Questions;
using PSK.WebApp.ViewModels;

namespace PSK.WebApp
{
	public static class AutoMapperConfiguration
	{
		public static void Configure()
		{
			Mapper.Initialize(cfg =>
			{
				cfg.CreateMap<Quiz, QuizViewModel>()
					.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
					.ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy.UserName))
					.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
				cfg.CreateMap<AbstractQuestion, QuestionListItemViewModel>()
					.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
					.ForMember(dest => dest.Mandatory, opt => opt.MapFrom(src => src.Mandatory))
					.ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.QuestionText))
					.ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.GetType().ToString()));
				cfg.CreateMap<ApplicationUser, ApplicationUserListItemViewModel>()
					.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
					.ForMember(dest => dest.Roles, opt => opt.ResolveUsing(src => ((IdentityRole)src.Roles.First()).Name));
				cfg.CreateMap<FreeTextQuestion, FreeTextAnswerViewModel>()
					.ForMember(dest => dest.Mandatory, opt => opt.MapFrom(src => src.Mandatory))
					.ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.QuestionText))
					.ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Id))
					.ForMember(dest => dest.Answer, opt => opt.Ignore());
				cfg.CreateMap<IntegerQuestion, IntegerAnswerViewModel>()
					.ForMember(dest => dest.Mandatory, opt => opt.MapFrom(src => src.Mandatory))
					.ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.QuestionText))
					.ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Id))
					.ForMember(dest => dest.UpperBound, opt => opt.MapFrom(src => src.UpperBound))
					.ForMember(dest => dest.LowerBound, opt => opt.MapFrom(src => src.LowerBound))
					.ForMember(dest => dest.Answer, opt => opt.Ignore());
				cfg.CreateMap<ChoiceOption, SelectListItem>()
					.ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
					.ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
					.ForMember(dest => dest.Disabled, opt => opt.Ignore())
					.ForMember(dest => dest.Group, opt => opt.Ignore())
					.ForMember(dest => dest.Selected, opt => opt.Ignore());
				cfg.CreateMap<SingleChoiceQuestion, SingleChoiceAnswerViewModel>()
					.ForMember(dest => dest.Mandatory, opt => opt.MapFrom(src => src.Mandatory))
					.ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.QuestionText))
					.ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Id))
					.ForMember(dest => dest.AnswerId, opt => opt.Ignore())
					.ForMember(dest => dest.Options, opt => opt.ResolveUsing(src =>
					{
						List<SelectListItem> result = new List<SelectListItem>();
						foreach (var choiceOption in src.Options)
						{
							result.Add(Mapper.Map<ChoiceOption, SelectListItem>(choiceOption));
						}
						return result;
					}));

                cfg.CreateMap<MultipleChoiceQuestion, MultiChoiceAnswerViewModel>()
                    .ForMember(dest => dest.Mandatory, opt => opt.MapFrom(src => src.Mandatory))
                    .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.QuestionText))
                    .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.AnswerId, opt => opt.Ignore())
                    .ForMember(dest => dest.Options, opt => opt.ResolveUsing(src =>
                    {
                        List<SelectListItem> result = new List<SelectListItem>();
                        foreach (var choiceOption in src.Options)
                        {
                            result.Add(Mapper.Map<ChoiceOption, SelectListItem>(choiceOption));
                        }
                        return result;
                    }));




            });

			Mapper.Configuration.AssertConfigurationIsValid();
		}
	}
}