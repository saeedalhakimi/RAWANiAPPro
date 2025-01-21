﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAWANi.WEBAPi.Domain.Models
{
    public enum ErrorCode
    {
        UnknownError = 999,
        NotFound = 1000,
        OperationCancelled = 1001,
        DatabaseError = 1002,
        InternalServerError = 500,
        InvalidInput = 1003,
        ConflictError = 1004,
        ValidationError = 1005,
        Unauthorized = 1006,
    }
}
