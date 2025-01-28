using Azure.Core;
using MediatR;
using RAWANi.WEBAPi.Application.Contracts.UserProfileDtos.Responses;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.UserProfileMDIR.Queries
{
    public class GetCurrentUserProfileQuery : IRequest<OperationResult<UserProfileResponseDto>>
    {
        public Guid UserProfileID { get; set; }
    }
}
