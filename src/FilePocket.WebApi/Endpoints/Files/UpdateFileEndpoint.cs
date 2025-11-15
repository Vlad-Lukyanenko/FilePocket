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
        private readonly IFileService _minioFileService;
        public UpdateFileEndpoint(IFileService minioFileService)
        {
            _minioFileService = minioFileService;
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
                await _minioFileService.UpdateFileAsync(request);
                await SendNoContentAsync(cancellationToken);
            }
            catch
            {
                await SendNotFoundAsync(cancellationToken);
            }
        }
    }
}
