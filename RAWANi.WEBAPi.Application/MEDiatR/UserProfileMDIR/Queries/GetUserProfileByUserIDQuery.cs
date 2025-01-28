using MediatR;
using RAWANi.WEBAPi.Application.Contracts.UserProfileDtos.Responses;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.UserProfileMDIR.Queries
{
    public class GetUserProfileByUserIDQuery : IRequest<OperationResult<IEnumerable<UserProfileResponseDto>>>
    {
        public string UserID {get; set;}
    }
}
