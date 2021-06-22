using GoldContactBookPro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldContactBookPro.Common
{
    public interface IJWT_TokenGenerator
    {
        public Task<string> GenerateToken(Users user);
    }
}
