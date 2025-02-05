using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.Contracts.PostDtos.Responses
{
    public record PostResponseDto
    {
        public Guid PostID { get; set; }
        public Guid UserProfileID { get; set; }
        public string PostTilte { get; set; }
        public string PostContent { get; set; }
        public string PostImageLink { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
