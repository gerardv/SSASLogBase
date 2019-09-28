using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSASLogBase.Data
{
    public class DBInitializer
    {
        public static void Initialize(DataContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}