using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace simple_aspnet_auth.demo
{
  public class UserService : IUserService
  {
    private const string Filename = "users.json";
    private readonly IList<User> _users = new List<User>();

    public UserService()
    {
      if (!File.Exists(Filename)) return;
      
      var json = File.ReadAllText(Filename);
      
      _users = JsonConvert.DeserializeObject<IList<User>>(json);

    }

    public User GetByName(string name)
    {
      var q = from x in _users where x.Name == name select x;
      var user = q.FirstOrDefault();

      return user;

    }

    public void Add(User user)
    {
      user.Id = _users.Count() + 1;

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
      var json = JsonConvert.SerializeObject(_users, Formatting.Indented);

      File.WriteAllText(Filename, json);

    }

  }

}