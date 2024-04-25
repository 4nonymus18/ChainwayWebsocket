using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFIDProject02.interfaces
{
    public interface IAutoReceive
    {
        bool Connect();
        void DisConnect();
    }
}
