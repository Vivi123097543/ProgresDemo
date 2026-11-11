
using System;
using System.Collections.Generic;
using ProgresDemo.Models;

namespace ProgresDemo.Services;

public interface IRateService
{
    IEnumerable<Rate> GetRates(DateTime date);
}

