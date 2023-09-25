using System.Net;

namespace MagicVilla_VillaAPI.Models;

public class APIResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccess { get; set; } = false;
    public List<string> ErrorMessages { get; set; }
    public object Result { get; set; }
}
