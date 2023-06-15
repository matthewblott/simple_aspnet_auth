namespace simple_aspnet_auth;

using JsonSerializer = System.Text.Json.JsonSerializer;

public class UserService : IUserService
{
  private const string Filename = "users.json";
  private readonly IList<User> _users = new List<User>();

  public UserService()
  {
    if (!File.Exists(Filename))
    {
      return;
    }

    var json = File.ReadAllText(Filename);

    _users = JsonSerializer.Deserialize<IList<User>>(json);

  }

  public User GetByName(string name) => _users.FirstOrDefault(u => u.Name == name);
  
}