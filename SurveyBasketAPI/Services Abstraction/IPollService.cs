using SurveyBasketAPI.Models;

namespace SurveyBasketAPI.Services_Abstraction;

public interface IPollService
{
    public IEnumerable<Poll> GetAll();

    public Poll? GetById(int id);

    public Poll AddPoll(Poll poll);

    public bool Update(int id,Poll poll);

    public bool Delete(int id);
}
