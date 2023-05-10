using Microsoft.AspNetCore.Identity;

namespace Bakalarska_prace.Models.Identity
{
    public class User : IdentityUser<int>
    {
        public User() : base() { }
        public User(string userName) : base(userName) { }

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
    }
}
