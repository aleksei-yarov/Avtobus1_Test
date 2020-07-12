using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Test_Bus.Hubs
{
    public class LinksHub: Hub
    {
        public async Task RedirectLink(int linkId)
        {
            await this.Clients.All.SendAsync("RedirectLink", linkId);
        }
    }
}
