﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class UpdateBUStatusResponse
    {
        public string? BUId { get; set; }
        public bool? IsActive { get; set; }
    }
}
