using Application.Common.Abstractions;

namespace Application.Common;

public class ApplicationConfiguration
{
    private readonly IRmwRepository _repo;

    public ApplicationConfiguration(IRmwRepository repo)
    {
        _repo = repo;
    }

    public async Task ConfigureAsync()
    {
        await Task.Run(Configure);
    }
    
    public void Configure()
    {
        
    }
}