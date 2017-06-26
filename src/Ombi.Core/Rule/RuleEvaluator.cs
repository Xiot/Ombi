﻿using Ombi.Core.Models.Requests;
using Ombi.Core.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ombi.Core.Models.Search;
using Ombi.Core.Rule.Interfaces;
using Ombi.Store.Entities.Requests;

namespace Ombi.Core.Rule
{
    public class RuleEvaluator : IRuleEvaluator
    {
        public RuleEvaluator(IServiceProvider provider)
        {
            RequestRules = new List<IRequestRules<BaseRequest>>();
            SearchRules = new List<IRequestRules<SearchViewModel>>();
            var baseSearchType = typeof(BaseRequestRule).FullName;
            var baseRequestType = typeof(BaseSearchRule).FullName;

            var ass = typeof(RuleEvaluator).GetTypeInfo().Assembly;

            foreach (var ti in ass.DefinedTypes)
            {
                if (ti?.BaseType?.FullName == baseSearchType)
                {
                    var type = ti?.AsType();
                    var ctors = type.GetConstructors();
                    var ctor = ctors.FirstOrDefault();

                    var services = new List<object>();
                    foreach (var param in ctor.GetParameters())
                    {
                        services.Add(provider.GetService(param.ParameterType));
                    }

                    var item = Activator.CreateInstance(type, services.ToArray());
                    RequestRules.Add((IRequestRules<BaseRequest>) item);
                }
            }
            
            foreach (var ti in ass.DefinedTypes)
            {
                if (ti?.BaseType?.FullName == baseRequestType)
                {
                    var type = ti?.AsType();
                    var ctors = type.GetConstructors();
                    var ctor = ctors.FirstOrDefault();

                    var services = new List<object>();
                    foreach (var param in ctor.GetParameters())
                    {
                        services.Add(provider.GetService(param.ParameterType));
                    }

                    var item = Activator.CreateInstance(type, services.ToArray());
                    SearchRules.Add((IRequestRules<SearchViewModel>) item);
                }
            }
        }

        private List<IRequestRules<BaseRequest>> RequestRules { get; }
        private List<IRequestRules<SearchViewModel>> SearchRules { get; }

        public async Task<IEnumerable<RuleResult>> StartRequestRules(BaseRequest obj)
        {
            var results = new List<RuleResult>();
            foreach (var rule in RequestRules)
            {
                var result = await rule.Execute(obj);
                results.Add(result);
            }

            return results;
        }

        public async Task<IEnumerable<RuleResult>> StartSearchRules(SearchViewModel obj)
        {
            var results = new List<RuleResult>();
            foreach (var rule in SearchRules)
            {
                var result = await rule.Execute(obj);
                results.Add(result);
            }

            return results;
        }
    }
}