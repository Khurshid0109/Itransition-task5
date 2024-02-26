using DataGenerator.Domain.Entities;

namespace DataGenerator.Service.Interfaces;
public interface IUserDataService
{
    public IEnumerable<User> GetUserData(int seed, double mistakeRate, string locale);
}
