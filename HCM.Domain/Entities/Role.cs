﻿using Microsoft.AspNetCore.Identity;

namespace HCM.Domain.Entities
{
	public class Role : IdentityRole<Guid>
	{
		public Role()
		{
		}
		public Role(string roleName) : base(roleName)
		{
		}
	}
}
