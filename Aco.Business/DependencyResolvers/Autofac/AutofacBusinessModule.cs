using Aco.Business.Abstract;
using Aco.Business.Concrete;
using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aco.Business.DependencyResolvers.Autofac
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AcoManager>().As<IAcoService>();
        }
    }
}
