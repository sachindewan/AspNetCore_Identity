﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Webapp.Data.Account
{
    public class User:IdentityUser
    {
        public string Department { get; set; }
        public string Position { get; set; }
    }
}
