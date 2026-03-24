namespace FitTrack_Pro.Common
{
    public class ResponseModel<T>
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

    }
}
