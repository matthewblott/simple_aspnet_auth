using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace simple_aspnet_auth;

public class UserService : IUserService
{
  private const string Filename = "users.json";
  private readonly IList<User> _users = new List<User>();

  public UserService()
  {
    if (File.Exists(Filename))
    {
      var json = File.ReadAllText(Filename);
      this._users = JsonConvert.DeserializeObject<IList<User>>(json);
    }

  }

  public User GetByName(string name)
  {
    var q = from x in this._users where x.Name == name select x;
    var user = q.FirstOrDefault();

    return user;

  }

  public void Add(User user)
  {
    user.Id = this._users.Count() + 1;

    this._users.Add(user);

    this.SaveChanges();

  }

  public void Update(User user)
  {
    this._users.Remove(this.GetByName(user.Name));
    this._users.Add(user);
    this.SaveChanges();

  }

  private void SaveChanges()
  {
    var json = JsonConvert.SerializeObject(this._users, Formatting.Indented);

    File.WriteAllText(Filename, json);

  }

}