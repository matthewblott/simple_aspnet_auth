using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace simple_aspnet_auth
{
  public class UserService : IUserService
  {
    const string filename = "users.json";
    IList<User> users = new List<User>();

    public UserService()
    {
      if (File.Exists(filename))
      {
        var json = File.ReadAllText(filename);
        this.users = JsonConvert.DeserializeObject<IList<User>>(json);
      }

    }

    public User GetByName(string name)
    {
      var q = from x in this.users where x.Name == name select x;
      var user = q.FirstOrDefault();

      return user;

    }

    public void Add(User user)
    {
      user.Id = this.users.Count() + 1;

      this.users.Add(user);

      this.SaveChanges();

    }

    public void Update(User user)
    {
      this.users.Remove(this.GetByName(user.Name));
      this.users.Add(user);
      this.SaveChanges();

    }

    void SaveChanges()
    {
      var json = JsonConvert.SerializeObject(this.users, Formatting.Indented);

      File.WriteAllText(filename, json);

    }

  }

}