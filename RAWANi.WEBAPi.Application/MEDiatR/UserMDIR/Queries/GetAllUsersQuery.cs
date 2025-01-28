using MediatR;
using RAWANi.WEBAPi.Application.Contracts.UsersDto.Responses;
using RAWANi.WEBAPi.Application.Models;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.UserMDIR.Queries
{
    public class GetAllUsersQuery : IRequest<OperationResult<PagedResponse<UserResponseDto>>>
    {
        public int PageNumber { get; set; } = 1; // Default to page 1
        public int PageSize { get; set; } = 10; // Default to 10 items per page
    }
}
