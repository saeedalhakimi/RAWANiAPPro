using MediatR;
using Microsoft.AspNetCore.Http;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.Commnads
{
    public class UpdatePostContentsCommand : IRequest<OperationResult<bool>>
    {
        public Guid PostID { get; set; }
        public Guid UserProfileID { get; set; }
        public string PostTitle { get; set; }
        public string PostContent { get; set; }
    }
}
