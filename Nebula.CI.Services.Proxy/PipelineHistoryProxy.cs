using System;
using System.Threading.Tasks;
using Nebula.CI.Services.Pipeline;
using Nebula.CI.Services.PipelineHistory;

namespace Nebula.CI.Services.Proxy
{
    public class PipelineHistoryProxy : IPipelineHistoryProxy
    {
        private readonly IPipelineHistoryAppService _pipelineHistoryAppService;

        public PipelineHistoryProxy(IPipelineHistoryAppService pipelineHistoryAppService)
        {
            _pipelineHistoryAppService = pipelineHistoryAppService;
        }

        public async Task CreateAsync(PipelineDto input)
        {
            var pipelineHistoryCreateDto = new PipelineHistoryCreateDto
            {
                No = input.ExecTimes,
                Diagram = input.Diagram,
                PipelineName = input.Diagram,
                PipelineId = input.Id
            };
            await _pipelineHistoryAppService.CreateAsync(pipelineHistoryCreateDto);
        }
    }
}
