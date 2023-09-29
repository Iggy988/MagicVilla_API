using MagicVilla_VillaAPI.Models;
using MagicVilla_WebProject.Models;

namespace MagicVilla_WebProject.Services.IServices;

public interface IBaseService
{
    APIResponse responseModel { get; set; }
    Task<T> SendAsync<T>(APIRequest apiRequest);
}
