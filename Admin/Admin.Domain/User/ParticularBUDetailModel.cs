﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class ParticularBUDetailModel
    {
        public string? BUName { get; set; }
        public string? BUHeadId { get; set; }
        public string? HierarchyLevelId { get; set; }
        public string? BUHeadName { get; set; }
        public string? HierarchyLevelName { get; set; }
        public bool? IsSales { get; set; }
    }
}