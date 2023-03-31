using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Models.Mappings
{
    public class NaceCodeMappings
    {
        public static NaceCode PostToNaceCode(NaceCodePostDto nacecodeDto)
        {
            var nacecode = new NaceCode
            {   
                UpdatedUser = nacecodeDto.UpdatedUser
            };

            return nacecode;
        }

        public static NaceCode PutToNaceCode(NaceCodePutDto nacecodeDto)
        {
            var nacecode = new NaceCode
            {
                NaceCodeID = nacecodeDto.NaceCodeID,
                Sector = nacecodeDto.Sector,
                Division = nacecodeDto.Division,
                Group = nacecodeDto.Group,
                Class = nacecodeDto.Class,
                Description = nacecodeDto.Description,
                Status = nacecodeDto.Status,
                UpdatedUser = nacecodeDto.UpdatedUser
            };

            return nacecode;
        }
    }
}