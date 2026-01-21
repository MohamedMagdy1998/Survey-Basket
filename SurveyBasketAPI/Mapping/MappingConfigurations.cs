using Mapster;
using SurveyBasketAPI.DTOs;
using SurveyBasketAPI.Entities;
using SurveyBasketAPI.Models;

namespace SurveyBasketAPI.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
      config.NewConfig<QuestionRequest, Question>()
            .Map(dest => dest.Answers, src => src.Answers.Select(answerContent => new Answer { Content = answerContent }));
        


    }
}
