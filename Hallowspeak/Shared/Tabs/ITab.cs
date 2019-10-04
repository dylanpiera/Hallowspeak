using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hallowspeak.Shared.Tabs
{
    public interface ITab
    {
        RenderFragment? ChildContent { get; }
    }
}
