using FilePocket.Contracts.Storage.Responses;
using FilePocket.Domain.Entities;
using FilePocket.Domain.Entities.Consumption;
using FilePocket.WebApi.Endpoints.Base;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using static System.Net.WebRequestMethods;

namespace FilePocket.WebApi.Endpoints.Storage
{
    public class GetUserStorageConsumptionEndpoint : BaseEndpointWithoutRequest<GetUserStorageConsumptionResponse>
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public GetUserStorageConsumptionEndpoint(IMapper mapper, UserManager<User> userManager)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        public override void Configure()
        {
            Verbs(Http.Get);
            Routes("api/storage/storageConsumption");
        }

        public override async Task HandleAsync(CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(UserId.ToString());
            var userStorage = (StorageConsumption)user.AccountConsumptions.Last();

            var response = _mapper.Map<GetUserStorageConsumptionResponse>(userStorage);

            await SendOkAsync(response, cancellationToken);
        }
    }
}
