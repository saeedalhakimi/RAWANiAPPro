using MediatR;
using Microsoft.AspNetCore.Http;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.Commnads
{
    public class UpdatePostImageCommand : IRequest<OperationResult<bool>>
    {
        public Guid PostID { get; set; }
        public Guid UserProfileID { get; set; }
        public IFormFile PostImage { get; set; }
    }
}
