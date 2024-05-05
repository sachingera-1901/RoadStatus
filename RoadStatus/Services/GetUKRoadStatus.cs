using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using RoadStatus.Contracts;
using RoadStatus.Exceptions;
using RoadStatus.Models;

namespace RoadStatus
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="httpClient">To make http calls to the Road Status Api</param>
    /// <param name="configuration">App configuration containing config values to make Api calls, app logging etc.</param>
    public class GetUKRoadStatus(HttpClient httpClient, IConfiguration configuration) : IGetRoadStatus
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _configuration = configuration;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsValid { get; set;}

        /// <summary>
        /// Given the road name, gets live status for the road by calling Road Status API.
        /// The URI specifics for the Road API are being picked up from application configuration.
        /// </summary>
        /// <param name="roadName">name of the road e.g. M2, A2 etc.</param>
        /// <returns>string containing live road status information</returns>
        /// <exception cref="ConfigurationException">If config value does not exist in app configuration</exception>
        public async Task<string> GetLiveRoadStatus(string? roadName){
            if(string.IsNullOrWhiteSpace(roadName)){
                return "Please provide a non-empty road name";
            }   
            try{
                var url = _configuration["RoadApiUrl"];
                url = !string.IsNullOrWhiteSpace(url) ? url : throw new ConfigurationException("Road Status API Url is not configured");
                url = $"{url}/{roadName}";

                var appId = _configuration["AppId"];
                appId = !string.IsNullOrWhiteSpace(appId) ? appId : throw new ConfigurationException("Road Status API App Id is not configured");

                var appKey = _configuration["AppKey"];
                appKey = !string.IsNullOrWhiteSpace(appKey) ? appKey : throw new ConfigurationException("Road Status API App Key is not configured");

                var query = new Dictionary<string, string?>
                {
                    { "app_id", appId },
                    { "app_key", appKey }
                };
                var response = await _httpClient.GetAsync(QueryHelpers.AddQueryString(url, query));

                return await BuildStatusText(response, roadName);
            }
            catch(Exception){
                //log exception
                return $"There's some problem getting the status of {roadName}. Please try again later.";
            }
        }

        private async Task<string> BuildStatusText(HttpResponseMessage response, string roadName){
            IsValid = false;

            if(response.StatusCode == HttpStatusCode.OK){
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var roadStatus = JsonSerializer.Deserialize<RoadStatusValid[]>(content, options);
                if(roadStatus == null || roadStatus.Length == 0){
                    return $"There's some problem getting the status of {roadName}. Please try again later.";
                }
                IsValid = true;
                return $"The status of the {roadStatus[0].DisplayName} is as follows\n        Road Status is {roadStatus[0].StatusSeverity}\n        Road Status Description is {roadStatus[0].StatusSeverityDescription}";
            }
            else{
                if(response.StatusCode == HttpStatusCode.NotFound){
                    return $"{roadName} is not a valid road";
                }
                
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var roadStatus = JsonSerializer.Deserialize<RoadStatusInvalid>(content, options);
                if(roadStatus == null || string.IsNullOrWhiteSpace(roadStatus.Message)){
                    return $"There's some problem getting the status of {roadName}. Please try again later.";
                }
                return roadStatus.Message;
            }
        }
    }
}