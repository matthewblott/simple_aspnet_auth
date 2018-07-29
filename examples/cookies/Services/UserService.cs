using System.Collections.Generic;
using System.Linq;

namespace simple_aspnet_auth
{
  public class UserService : IUserService
  {
    IList<User> users;

    public UserService()
    {
      this.CreateList();
    }

    public User GetByName(string name)
    {
      var q = from x in this.users where x.Name == name select x;
      var user = q.FirstOrDefault();

      return user;

    }

    void CreateList()
    {
      const string password = "password";

      this.users = new List<User>
      {
        new User
        {
          Id = 1,
          Name = "admin",
          Password = password,
          Groups = new List<Group>
          {
            new Group
            {
              Id = 1,
              Name = GroupNames.Admins
            },
            new Group
            {
              Id = 2,
              Name = GroupNames.SuperUsers
            },
            new Group
            {
              Id = 3,
              Name = GroupNames.Users
            },

          }
        },
        new User
        {
          Id = 2,
          Name = "superuser",
          Password = password,
          Groups = new List<Group>
          {
            new Group
            {
              Id = 2,
              Name = GroupNames.SuperUsers
            },
            new Group
            {
              Id = 3,
              Name = GroupNames.Users
            },

          }
        },
        new User
        {
          Id = 3,
          Name = "john",
          Password = password,
          Groups = new List<Group>
          {
            new Group
            {
              Id = 3,
              Name = GroupNames.Users
            },

          }
        },
        new User
        {
          Id = 4,
          Name = "jane",
          Password = password,
          Groups = new List<Group>
          {
            new Group
            {
              Id = 3,
              Name = GroupNames.Users
            },

          }
        },

      };

    }

  }

}