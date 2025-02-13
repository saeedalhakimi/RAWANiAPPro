using MediatR;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.Commnads
{
    public class DeletePostCommentCommand : IRequest<OperationResult<bool>>
    {
        public Guid CommentID { get; set; }
        public Guid UserProfileID { get; set; }
    }
}
