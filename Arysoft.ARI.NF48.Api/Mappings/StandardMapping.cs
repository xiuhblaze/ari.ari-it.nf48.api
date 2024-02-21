using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class StandardMapping
    {
        public static Standard ItemPutDtoToStandard(StandardPutDto itemDto) 
        {
            var item = new Standard();

            item.ID = itemDto.StandardID;
            item.Name = itemDto.Name;
            item.Description = itemDto.Description;
            item.MaxReductionsDays = itemDto.MaxReductionsDays;
            item.Status = itemDto.Status;
            item.UpdatedUser = item.UpdatedUser;

            return item;
        }
    }
}