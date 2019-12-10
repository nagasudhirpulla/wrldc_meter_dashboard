﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MeterDataDashboard.Core.Common
{
    // This can easily be modified to be BaseEntity<T> and public T Id to support different key types.
    // Using non-generic integer types for simplicity and to ease caching logic
    public class BaseEntity
    {
        public int Id { get; set; }
    }
}
