using System.Collections.Generic;

namespace simple_aspnet_auth
{
  public class User
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public IList<Group> Groups { get; set; }
  }
}