namespace simple_aspnet_auth.demo
{
  public interface IUserService
  {
    User GetByName(string name);
    void Add(User user);
    void Update(User user);
  }
}