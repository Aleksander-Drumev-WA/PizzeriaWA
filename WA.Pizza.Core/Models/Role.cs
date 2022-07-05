using Microsoft.AspNetCore.Identity;

namespace WA.Pizza.Core.Models
{
	public class Role : IdentityRole<int>
	{
		public Role() : base()
		{

		}

		public Role(string roleName) : base(roleName)
		{

		}
	}
}
