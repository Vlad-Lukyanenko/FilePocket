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
    public class GetFileEndpoint : BaseEndpointWithoutRequest<FileResponseModel>
    {
        private readonly IServiceManager _service;

        public GetFileEndpoint(IServiceManager service)
        {
            _service = service;
        }

        public override void Configure()
        {
            Get("api/files/{fileId:guid}");
            AuthSchemes("Bearer");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var fileId = Route<Guid>("fileId");
            var file = await _service.FileService.GetFileByUserIdAndIdAsync(UserId, fileId);

            if (file == null)
            {
                await SendNotFoundAsync(cancellationToken);
                return;
            }

            await SendOkAsync(file, cancellationToken);
        }
    }
}
