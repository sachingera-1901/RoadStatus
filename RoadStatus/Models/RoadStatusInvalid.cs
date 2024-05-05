namespace RoadStatus.Models
{
    public class RoadStatusInvalid
    {
        public string? ExceptionType {get; set;}

        public int HttpStatusCode {get; set;}

        public string? HttpStatus {get; set;}

        public string? Message {get; set;}
    }
}