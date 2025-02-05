﻿using MediatR;
using RAWANi.WEBAPi.Application.Contracts.PostDtos.Responses;
using RAWANi.WEBAPi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Application.MEDiatR.PostsMDIR.Queries
{
    public class GetPostByIDQuery : IRequest<OperationResult<PostResponseDto>>
    {
        public Guid PostID { get; set; }
    }
}
