﻿using Intermech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGM.IPS.Client.ObjectCreator.BusinessLogic
{
    class IpsObjectAttributesUtil
    {
        public static readonly Guid Designation = new Guid(SystemGUIDs.attributeDesignation);

        public static readonly Guid Name = new Guid(SystemGUIDs.attributeName);
    }
}
