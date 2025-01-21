using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string IdentityId { get; set; } 
        public bool IsUsed { get; set; } = false;
        public bool IsRevoked { get; set; } = false;
    }
}
