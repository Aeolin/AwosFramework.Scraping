using AwosFramework.Scraping.Binding;
using AwosFramework.Scraping.Binding.DefaultBinders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Hosting.Builders
{
    public class BinderFactoryBuilder
    {
        private readonly List<IBinderGenerator> _generators = new List<IBinderGenerator>();

        public BinderFactoryBuilder AddInbuiltBinders()
        {
            AddBinder<BodyBinderGenerator>();
            AddBinder<HtmlBinderGenerator>();
            AddBinder<JobBinderGenerator>();
            AddBinder<QueryBinderGenerator>();
            AddBinder<RouteBinderGenerator>();
            return this;
        }

        public BinderFactoryBuilder AddBinder<T>(Action<T> configure = null) where T : IBinderGenerator, new()
        {
            var generator = new T();
            configure?.Invoke(generator);
            _generators.Add(generator);
            return this;
        }

        public BinderFactoryBuilder AddBinder(IBinderGenerator generator)
        {
            _generators.Add(generator);
            return this;
        }

        public IBinderFactory Build()
        {
            return new BinderFactory(_generators.ToArray());
        }

    }
}
