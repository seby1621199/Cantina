using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Exceptions;

public class DeliveryException : Exception
{
    public DeliveryException(string message) : base(message) { }
}
