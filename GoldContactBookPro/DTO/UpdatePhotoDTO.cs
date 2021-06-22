using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldContactBookPro.DTO
{
    public class UpdatePhotoDTO
    {
        public IFormFile PhotoUrl { get; set; }
    }
}
