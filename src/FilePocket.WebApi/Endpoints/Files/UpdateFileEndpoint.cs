using FilePocket.Application.Interfaces.Services;
using FilePocket.Domain.Models;
using FilePocket.WebApi.Endpoints.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilePocket.WebApi.Endpoints.Files
{
    public class UpdateFileEndpoint : BaseEndpointWithoutResponse<UpdateFileModel>
    {
        private readonly IServiceManager _service;
        public UpdateFileEndpoint(IServiceManager service)
        {
            _service = service;
        }
        public override void Configure()
        {
            Put("api/files");
            AuthSchemes("Bearer");
        }
        public override async Task HandleAsync(UpdateFileModel request, CancellationToken cancellationToken)
        {
            request.UserId = UserId;

            try
            {
                await _service.FileService.UpdateFileAsync(request);
                await SendNoContentAsync(cancellationToken);
            }
            catch
            {
                await SendNotFoundAsync(cancellationToken);
            }
        }
    }
}
