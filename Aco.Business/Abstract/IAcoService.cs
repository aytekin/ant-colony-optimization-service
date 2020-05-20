using Aco.Entity;
using Aco.Entity.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aco.Business.Abstract
{
    public interface IAcoService
    {
        AntResult Calculate(List<City> cities, AcoOptions acoOptions);
    }
}
