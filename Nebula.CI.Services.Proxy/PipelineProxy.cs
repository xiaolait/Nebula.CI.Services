using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nebula.CI.Services.Pipeline;
using Nebula.CI.Services.PipelineHistory;

namespace Nebula.CI.Services.Proxy
{
    /*
    public class PipelineProxy : IPipelineProxy
    {
        private readonly IPipelineAppService _pipelineAppService;

        public PipelineProxy(IPipelineAppService pipelineAppService)
        {
            _pipelineAppService = pipelineAppService;
        }

        public async Task<List<int>> GetIdListAsync()
        {
            var pipelineList = await _pipelineAppService.GetListAsync();
            return pipelineList.Select(p => p.Id).ToList();
        }

        public async Task UpdateStatusAsync(int id, string status, string time)
        {
            var updatePipelineStatusDto = new UpdatePipelineStatusDto
            {
                Id = id,
                Status = status,
                Time = time
            };
            await _pipelineAppService.UpdateStatusAsync(updatePipelineStatusDto);
        }
    }
    */
}