namespace Sahityak.Models
{
    public class Response
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Response(bool success, string message)
        {
            this.Success = success;
            this.Message = message;
        }
    }
    public class PostResponse : Response
    {
        public PostResponse(int id, bool success, string msg) : base(success, msg)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
