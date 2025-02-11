using Arysoft.ARI.NF48.Api.CustomEntities;

namespace Arysoft.ARI.NF48.Api.Response
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public Metadata Meta { get; set; }

        public ApiResponse(T data)
        {
            Data = data;
        }

        public ApiResponse()
        {
        }
    }
}