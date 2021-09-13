using AudiosBot.Domain.Filters;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AudiosBot.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BasicAuthAttribute : TypeFilterAttribute
    {
        public BasicAuthAttribute() : base(typeof(BasicAuthFilter))
        {

        }
    }
}
