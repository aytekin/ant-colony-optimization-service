using Aco.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aco.Business.Abstract
{
    public interface IAcoService
    {
        AntResult Calculate(List<City> cities);
    }
}
