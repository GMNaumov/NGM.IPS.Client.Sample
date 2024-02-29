using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGM.IPS.Client.ObjectCreator.BusinessLogic
{
    interface IObjectCreator
    {
        int CreateNewObject(Guid objectType, string objectDesignation, string objectName);
    }
}
