﻿using FilePocket.Domain.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace FilePocket.Application.IntegrationTests.Common;

public abstract class FilePocketWebAppTest(FilePocketWebAppFactory factory) : IClassFixture<FilePocketWebAppFactory>
{
    protected readonly FilePocketWebAppFactory FilePocketWebAppFactory = factory;

    protected readonly AccountConsumptionConfigurationModel AccountConsumptionSettings =
        factory.Services.CreateScope().ServiceProvider.GetRequiredService<IOptions<AccountConsumptionConfigurationModel>>().Value;
}