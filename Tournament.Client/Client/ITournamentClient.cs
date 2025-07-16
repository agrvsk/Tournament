namespace Tournament.Client.Client
{
    public interface ITournamentClient
    {
        Task<T> GetAsync<T>(string path, string contentType = "application/json");
    }
}