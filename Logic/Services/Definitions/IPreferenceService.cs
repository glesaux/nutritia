﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public interface IPreferenceService
    {
        IList<Preference> RetrieveAll();
        Preference Retrieve(RetrievePreferenceArgs args);
    }
}
