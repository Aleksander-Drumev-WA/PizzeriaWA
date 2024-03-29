﻿using WA.Pizza.Core.ModelConfig;

namespace WA.Pizza.Core.Models
{
	public class RefreshToken : BaseEntity
	{
		public string Token { get; set; } = null!;

		public string JwtId { get; set; } = null!;

		public bool IsUsed { get; set; }

		public bool IsRevoked { get; set; }

		public DateTime? LastModifiedOn { get; set; }

		public DateTime ExpiryDate { get; set; }

		public int UserId { get; set; }

		public virtual User User { get; set; } = null!;
	}
}
