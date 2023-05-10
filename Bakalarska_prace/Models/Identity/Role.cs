using Microsoft.AspNetCore.Identity;

namespace Bakalarska_prace.Models.Identity
{
    public class Role :IdentityRole<int>
    {
        public Role() : base() { }
        public Role(string userName) : base(userName) { }
    }
}
