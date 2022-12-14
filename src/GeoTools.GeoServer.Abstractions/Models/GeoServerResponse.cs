namespace GeoTools.GeoServer.Models
{
    public class GeoServerResponse<T>
    {
        public int StatusCode { get; }

        public T Response { get; }

        public GeoServerResponse(int statusCode, T response)
        {
            StatusCode = statusCode;
            Response = response;
        }
    }
}
