using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrewItBackEnd.Dtos
{
    public class CartDeleteRequest
    {
    public string UserId { get; set; }
    public int ProductId { get; set; }
    }
}