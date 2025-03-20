namespace SmartLeadsPortalDotNetApi.Entities
{
    public class DefaultResponse
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        public int? Id { get; set; }

        public static DefaultResponse FailedResponse(string message)
        {
            var response = new DefaultResponse();
            response.Result = false;
            response.Message = message;
            return response;
        }

        public static DefaultResponse SucceedResponse(string message)
        {
            var response = new DefaultResponse();
            response.Result = true;
            response.Message = message;
            return response;
        }
    }
}
