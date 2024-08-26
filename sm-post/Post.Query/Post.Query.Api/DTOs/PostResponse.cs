using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Post.Common.DTOs;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.DTOs
{
    public class PostResponse : BaseResponse
    {
        public IList<PostEntity> Posts { get; set; }
    }
}
