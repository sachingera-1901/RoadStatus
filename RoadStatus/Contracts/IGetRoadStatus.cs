namespace RoadStatus.Contracts
{
    /// <summary>
    /// Exposes methods for getting Live Road Status
    /// </summary>
    public interface IGetRoadStatus
    {
        /// <summary>
        /// Given the road name, gets live status for the road 
        /// </summary>
        /// <param name="roadName">name of the road e.g. M2, A2 etc.</param>
        /// <returns>string containing live road status information</returns>
        Task<string> GetLiveRoadStatus(string? roadName);

        /// <summary>
        /// Indicates if getting live road status has been able to generate a successful status message or not
        /// </summary>
        bool IsValid {get; internal set;}
    }
}