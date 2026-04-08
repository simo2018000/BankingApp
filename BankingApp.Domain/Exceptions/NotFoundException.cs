using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entity, Guid id)
            : base($"{entity} with ID {id} was not found.")
        {
        }
    }
}