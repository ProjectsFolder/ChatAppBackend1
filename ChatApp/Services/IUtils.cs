using ChatApp.Models;
using System;

namespace ChatApp.Services
{
    public interface IUtils
    {
        Guid GetGiudByUser();
        User GetUserByToken();
    }
}
