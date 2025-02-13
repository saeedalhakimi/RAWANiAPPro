using MediatR;
using RAWANi.WEBAPi.Application.Contracts.PostDtos.Responses;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.Queries
{
    public class GetPostWithCommentsQuery : IRequest<OperationResult<PostWithCommentsResponseDto>>
    {
        public Guid PostID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
    }
}
