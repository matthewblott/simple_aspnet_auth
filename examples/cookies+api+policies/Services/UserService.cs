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

  public void Add(User user)
  {
    user.Id = _users.Count + 1;
  
    _users.Add(user);
  
    SaveChanges();
  
  }
  
  public void Update(User user)
  {
    _users.Remove(GetByName(user.Name));
    _users.Add(user);
    SaveChanges();
  
  }

  private void SaveChanges()
  {
    var json = JsonSerializer.Serialize(_users);
    
    File.WriteAllText(Filename, json);
  
  }
  
}
