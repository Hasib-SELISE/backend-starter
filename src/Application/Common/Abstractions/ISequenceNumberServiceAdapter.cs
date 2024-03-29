﻿using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Abstractions
{
    public interface ISequenceNumberServiceAdapter
    {
        Task<SequenceNumberQueryResponse> GetSequenceNumberAsync(string context);
    }
}
