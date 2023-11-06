﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackIpProject.Interfaces
{
    public interface IIPInfoProvider
    {
        Task<IIPDetails> GetIPDetailsAsync(string ip);
    }
}
